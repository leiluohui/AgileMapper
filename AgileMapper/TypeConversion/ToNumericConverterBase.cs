﻿namespace AgileObjects.AgileMapper.TypeConversion
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using NetStandardPolyfills;

    internal abstract class ToNumericConverterBase : TryParseConverterBase
    {
        private static readonly Type[] _handledSourceTypes =
            Constants.NumericTypes
                .Concat(typeof(string), typeof(char), typeof(object))
                .ToArray();

        protected ToNumericConverterBase(ToStringConverter toStringConverter, Type numericType)
            : base(toStringConverter, numericType)
        {
        }

        protected override bool CanConvert(Type nonNullableSourceType)
        {
            return base.CanConvert(nonNullableSourceType) ||
                   nonNullableSourceType.IsEnum() ||
                   _handledSourceTypes.Contains(nonNullableSourceType);
        }

        public override Expression GetConversion(Expression sourceValue, Type targetType)
        {
            var sourceType = GetNonEnumSourceType(sourceValue);

            if (IsCoercible(sourceType))
            {
                if (!targetType.IsWholeNumberNumeric())
                {
                    sourceValue = sourceValue.GetConversionTo(sourceType);
                }

                return sourceValue.GetConversionTo(targetType);
            }

            return IsNonNumericType(sourceValue.Type)
                ? base.GetConversion(sourceValue, targetType)
                : GetCheckedNumericConversion(sourceValue, targetType);
        }

        private static Type GetNonEnumSourceType(Expression sourceValue)
        {
            return sourceValue.Type.IsEnum() ? Enum.GetUnderlyingType(sourceValue.Type) : sourceValue.Type;
        }

        private static bool IsNonNumericType(Type type)
        {
            return (type == typeof(string)) ||
                   (type == typeof(object)) ||
                   (type == typeof(char)) ||
                   (type == typeof(char?));
        }

        protected abstract bool IsCoercible(Type sourceType);

        private static Expression GetCheckedNumericConversion(Expression sourceValue, Type targetType)
        {
            var numericValueIsValid = GetNumericValueValidityCheck(sourceValue, targetType);
            var castSourceValue = sourceValue.GetConversionTo(targetType);
            var defaultTargetType = targetType.ToDefaultExpression();
            var inRangeValueOrDefault = Expression.Condition(numericValueIsValid, castSourceValue, defaultTargetType);

            return inRangeValueOrDefault;
        }

        private static Expression GetNumericValueValidityCheck(Expression sourceValue, Type targetType)
        {
            var nonNullableTargetType = targetType.GetNonNullableType();
            var numericValueIsInRange = NumericValueIsInRangeComparison.For(sourceValue, nonNullableTargetType);

            if (NonWholeNumberCheckIsNotRequired(sourceValue, nonNullableTargetType))
            {
                return numericValueIsInRange;
            }

            var one = GetConstantValue(1, sourceValue);
            var sourceValueModuloOne = Expression.Modulo(sourceValue, one);
            var zero = GetConstantValue(0, sourceValue);
            var moduloOneEqualsZero = Expression.Equal(sourceValueModuloOne, zero);

            return Expression.AndAlso(numericValueIsInRange, moduloOneEqualsZero);
        }

        private static bool NonWholeNumberCheckIsNotRequired(Expression sourceValue, Type nonNullableTargetType)
        {
            return sourceValue.Type.IsEnum() ||
                   sourceValue.Type.IsWholeNumberNumeric() ||
                   !nonNullableTargetType.IsWholeNumberNumeric();
        }

        private static Expression GetConstantValue(int value, Expression sourceValue)
        {
            var constant = value.ToConstantExpression();

            return (sourceValue.Type != typeof(int))
                ? constant.GetConversionTo(sourceValue.Type) : constant;
        }
    }
}