﻿namespace AgileObjects.AgileMapper.Api.Configuration
{
    using System;
    using System.Linq.Expressions;
    using Members;

    internal class MappingConfigInfo
    {
        private static readonly Type _allSourceTypes = typeof(MappingConfigInfo);
        private static readonly string _allRuleSets = Guid.NewGuid().ToString();

        private Type _sourceType;
        private Type _targetType;
        private Type _sourceValueType;
        private string _mappingRuleSetName;
        private ConfiguredLambdaInfo _conditionLambda;
        private bool _negateCondition;

        public MappingConfigInfo(MapperContext mapperContext)
        {
            MapperContext = mapperContext;
        }

        public GlobalContext GlobalContext => MapperContext.GlobalContext;

        public MapperContext MapperContext { get; }

        public MappingConfigInfo ForAllSourceTypes() => ForSourceType(_allSourceTypes);

        public MappingConfigInfo ForSourceType<TSource>() => ForSourceType(typeof(TSource));

        public MappingConfigInfo ForSourceType(Type sourceType)
        {
            _sourceType = sourceType;
            return this;
        }

        public MappingConfigInfo ForAllTargetTypes() => ForTargetType<object>();

        public MappingConfigInfo ForTargetType<TTarget>() => ForTargetType(typeof(TTarget));

        public MappingConfigInfo ForTargetType(Type targetType)
        {
            _targetType = targetType;
            return this;
        }

        public bool HasSameSourceTypeAs(MappingConfigInfo otherConfigInfo) => _sourceType == otherConfigInfo._sourceType;

        public bool IsForSourceType(MappingConfigInfo otherConfigInfo) => IsForSourceType(otherConfigInfo._sourceType);

        public bool IsForSourceType(Type sourceType)
            => (_sourceType == _allSourceTypes) || _sourceType.IsAssignableFrom(sourceType);

        public bool HasSameTargetTypeAs(MappingConfigInfo otherConfigInfo) => _targetType == otherConfigInfo._targetType;

        public bool IsForTargetType(MappingConfigInfo otherConfigInfo) => IsForTargetType(otherConfigInfo._targetType);

        public bool IsForTargetType(Type targetType) => _targetType.IsAssignableFrom(targetType);

        public MappingConfigInfo ForAllRuleSets() => ForRuleSet(_allRuleSets);

        public MappingConfigInfo ForRuleSet(string name)
        {
            _mappingRuleSetName = name;
            return this;
        }

        public bool IsForRuleSet(string mappingRuleSetName)
        {
            return (_mappingRuleSetName == _allRuleSets) ||
                (mappingRuleSetName == _mappingRuleSetName);
        }

        public MappingConfigInfo ForSourceValueType(Type sourceValueType)
        {
            _sourceValueType = sourceValueType;
            return this;
        }

        public void ThrowIfSourceTypeDoesNotMatch<TTargetValue>()
        {
            MapperContext.ValueConverters.ThrowIfUnconvertible(_sourceValueType, typeof(TTargetValue));
        }

        #region Conditions

        public bool HasCondition => _conditionLambda != null;

        public void AddCondition(LambdaExpression conditionLambda)
        {
            _conditionLambda = ConfiguredLambdaInfo.For(conditionLambda);
        }

        public void NegateCondition()
        {
            if (HasCondition)
            {
                _negateCondition = true;
            }
        }

        public Expression GetConditionOrNull(IMemberMappingContext context)
        {
            if (!HasCondition)
            {
                return null;
            }

            var contextualisedCondition = _conditionLambda.GetBody(context);

            if (_negateCondition)
            {
                contextualisedCondition = Expression.Not(contextualisedCondition);
            }

            return contextualisedCondition;
        }

        #endregion
    }
}