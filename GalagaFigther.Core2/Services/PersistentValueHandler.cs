using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IPersistentValueHandler : IClearable
    {
        void RegisterGradientValue(GameObject instance1, Expression<Func<GameObject, float>> property1, GameObject instance2, Expression<Func<GameObject, float>> property2, float period);
        void RegisterRange<T>(GameObject instance, Expression<Func<GameObject, T>> property, T min, T max)
            where T : IComparable;
        
        void RegisterRange(GameObject instance, Expression<Func<GameObject, Vector2>> property, Vector2 min, Vector2 max);

        void Update(float frameTime);
    }
    public class PersistentValueHandler : IPersistentValueHandler
    {
        private Dictionary<GameObject, List<Action<GameObject>>> _registry = [];

        private List<GradientAction> _gradientActions = [];

        public void Clear()
        {
            _gradientActions.Clear();
            _registry.Clear();
        }

        public void RegisterRange<T>(GameObject instance, Expression<Func<GameObject, T>> property, T min, T max)
            where T : IComparable
        {
            // Compile the expression to get a getter
            var getter = property.Compile();
            
            // Create a setter action that handles both simple and nested properties
            Action<GameObject, T> setter = CreateSetter<T>(property);
            
            // Create the clamping action
            Action<GameObject> clampAction = (gameObject) =>
            {
                var currentValue = getter(gameObject);
                
                T clampedValue = currentValue;
                
                // Clamp to minimum
                if (currentValue.CompareTo(min) < 0)
                    clampedValue = min;
                // Clamp to maximum
                else if (currentValue.CompareTo(max) > 0)
                    clampedValue = max;
                
                // Set the clamped value back
                if (!clampedValue.Equals(currentValue))
                    setter(gameObject, clampedValue);
            };
            
            if(!_registry.ContainsKey(instance))
                _registry.Add(instance, []);

            _registry[instance].Add(clampAction);
        }

        public void RegisterRange(GameObject instance, Expression<Func<GameObject, Vector2>> property, Vector2 min, Vector2 max)
        {
            // Compile the expression to get a getter
            var getter = property.Compile();
            
            // Create a setter action for Vector2
            Action<GameObject, Vector2> setter = CreateVector2Setter(property);
            
            // Create the clamping action for Vector2
            Action<GameObject> clampAction = (gameObject) =>
            {
                var currentValue = getter(gameObject);
                
                // Clamp X and Y components separately
                var clampedX = Math.Clamp(currentValue.X, min.X, max.X);
                var clampedY = Math.Clamp(currentValue.Y, min.Y, max.Y);
                
                var clampedValue = new Vector2(clampedX, clampedY);
                
                // Set the clamped value back if it changed
                if (!clampedValue.Equals(currentValue))
                    setter(gameObject, clampedValue);
            };
            
            if(!_registry.ContainsKey(instance))
                _registry.Add(instance, []);

            _registry[instance].Add(clampAction);
        }

        public void RegisterGradientValue(GameObject instance1, Expression<Func<GameObject, float>> property1, GameObject instance2, Expression<Func<GameObject, float>> property2, float period)
        {
            var pi1 = GetProperty(property1);
            var pi2 = GetProperty(property2);
            var lifetime = 0f;
            var original1 = (float)pi1.GetValue(instance1);
            var original2 = (float)pi2.GetValue(instance2);
            var originalDistance = original1 - original2;

            var action = new GradientAction();
            action.Action = frameTime =>
            {
                lifetime += frameTime;

                if (lifetime > period)
                {
                    action.Deactivated = true;
                    return;
                }

                var val2 = (float)pi2.GetValue(instance2);
                var pct = (1 - lifetime / period);
                var newDistance = pct * originalDistance;
                var newValue = val2 + newDistance;
                pi1.SetValue(instance1, newValue);
            };

            _gradientActions.Add(action);
        }

        private static PropertyInfo GetProperty<T>(Expression<Func<GameObject, T>> property)
        {
            var parameter = property.Parameters[0];
            var valueParameter = Expression.Parameter(typeof(T), "value");

            // Handle both simple properties and nested struct properties
            if (property.Body is MemberExpression memberExpr)
            {
                // Simple property access like p => p.X
                if (memberExpr.Member is PropertyInfo propInfo && propInfo.CanWrite)
                {
                    return propInfo;
                }
            }

            return null;
        }
        
        private static Action<GameObject, T> CreateSetter<T>(Expression<Func<GameObject, T>> property)
        {
            var parameter = property.Parameters[0];
            var valueParameter = Expression.Parameter(typeof(T), "value");
            
            // Handle both simple properties and nested struct properties
            if (property.Body is MemberExpression memberExpr)
            {
                // Simple property access like p => p.X
                if (memberExpr.Member is PropertyInfo propInfo && propInfo.CanWrite)
                {
                    var assign = Expression.Assign(memberExpr, valueParameter);
                    var lambda = Expression.Lambda<Action<GameObject, T>>(assign, parameter, valueParameter);
                    return lambda.Compile();
                }
                
                // Handle nested property access like p => p.Rect.X
                if (memberExpr.Expression is MemberExpression parentMemberExpr &&
                    memberExpr.Member is PropertyInfo nestedPropInfo &&
                    parentMemberExpr.Member is PropertyInfo parentPropInfo &&
                    parentPropInfo.PropertyType.IsValueType) // Struct
                {
                    return (gameObject, value) =>
                    {
                        // Get the current struct value
                        var structValue = parentPropInfo.GetValue(gameObject);
                        
                        // Set the nested property on the struct
                        nestedPropInfo.SetValue(structValue, value);
                        
                        // Set the modified struct back to the parent property
                        parentPropInfo.SetValue(gameObject, structValue);
                    };
                }
            }
            
            throw new ArgumentException("Unsupported property expression. Use simple properties or properties of struct values.", nameof(property));
        }

        private static Action<GameObject, Vector2> CreateVector2Setter(Expression<Func<GameObject, Vector2>> property)
        {
            var parameter = property.Parameters[0];
            var valueParameter = Expression.Parameter(typeof(Vector2), "value");
            
            if (property.Body is MemberExpression memberExpr)
            {
                // Simple Vector2 property access like p => p.Speed
                if (memberExpr.Member is PropertyInfo propInfo && propInfo.CanWrite)
                {
                    var assign = Expression.Assign(memberExpr, valueParameter);
                    var lambda = Expression.Lambda<Action<GameObject, Vector2>>(assign, parameter, valueParameter);
                    return lambda.Compile();
                }
            }
            
            throw new ArgumentException("Unsupported Vector2 property expression. Use simple Vector2 properties.", nameof(property));
        }

        public void Update(float frameTime)
        {
            foreach (var registry in _registry)
            {
                foreach(var action in registry.Value)
                {
                    action(registry.Key);
                }
            }

            foreach(var gradientAction in _gradientActions)
            {
                gradientAction.Action(frameTime);
            }

            _gradientActions.RemoveAll(x => x.Deactivated);
        }


        private class GradientAction
        {
            public bool Deactivated { get; set; }
            public Action<float> Action { get; set; }
        }
    }
}
