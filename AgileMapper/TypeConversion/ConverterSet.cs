﻿namespace AgileObjects.AgileMapper.TypeConversion
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Api.Configuration;
    using Extensions;

    internal class ConverterSet
    {
        private readonly IValueConverter[] _converters;

        public ConverterSet()
        {
            _converters = new IValueConverter[]
            {
                new ToStringConverter(),
                //new ToDateTimeConverter(),
                new ToEnumConverter(),
                new ToNumericConverter<int>(),
                new ToNumericConverter<long>(),
                new DefaultTryParseConverter<Guid>()
            };
        }

        public void ThrowIfUnconvertible(Type sourceValueType, Type targetValueType)
        {
            if (targetValueType.IsAssignableFrom(sourceValueType))
            {
                return;
            }

            if (GetConverterFor(targetValueType, sourceValueType) != null)
            {
                return;
            }

            throw new MappingConfigurationException(
                $"Unable to convert configured {sourceValueType.Name} to target type {targetValueType.Name}");
        }

        private IValueConverter GetConverterFor(Type targetValueType, Type sourceValueType)
        {
            targetValueType = targetValueType.GetNonNullableUnderlyingTypeIfAppropriate();

            return _converters.FirstOrDefault(c =>
                c.IsFor(targetValueType) && c.CanConvert(sourceValueType));
        }

        public Expression GetConversion(Expression sourceValue, Type targetType)
        {
            if (sourceValue.Type == targetType)
            {
                return sourceValue;
            }

            if (targetType.IsAssignableFrom(sourceValue.Type))
            {
                return Expression.Convert(sourceValue, targetType);
            }

            var converter = GetConverterFor(targetType, sourceValue.Type);

            return converter.GetConversion(sourceValue, targetType);
        }
    }
}