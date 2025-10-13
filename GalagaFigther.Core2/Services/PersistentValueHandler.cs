using GalagaFigther.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services
{
    public interface IPersistentValueHandler
    {
        void RegisterRange<T>(GameObject instance, Expression<Func<GameObject, T>> property, T min, T max)
            where T : IComparable;
        
        void RegisterRange(GameObject instance, Expression<Func<GameObject, Vector2>> property, Vector2 min, Vector2 max);

        void Update();
    }
    public class PersistentValueHandler : IPersistentValueHandler
    {
        private Dictionary<GameObject, List<Action<GameObject>>> _registry = [];

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

        public void Update()
        {
            foreach (var registry in _registry)
            {
                foreach(var action in registry.Value)
                {
                    action(registry.Key);
                }
            }
        }
    }
}
