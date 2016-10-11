namespace AgileObjects.AgileMapper.ObjectPopulation
{
    using System;
    using System.Linq.Expressions;
    using Caching;
    using Extensions;
    using Members;

    internal class MemberMappingContextData<TSource, TTarget> : IMemberMappingContextData
    {
        private readonly ObjectMappingContextData<TSource, TTarget> _parent;
        private readonly ICache<IQualifiedMember, Func<TSource, Type>> _runtimeTypeGettersCache;

        public MemberMappingContextData(ObjectMappingContextData<TSource, TTarget> parent)
        {
            _parent = parent;
            _runtimeTypeGettersCache = parent.MappingContext.MapperContext.Cache.CreateScoped<IQualifiedMember, Func<TSource, Type>>();
        }

        public MappingRuleSet RuleSet => _parent.RuleSet;

        public IQualifiedMember SourceMember => MapperData.SourceMember;

        public QualifiedMember TargetMember => MapperData.TargetMember;

        public MemberMapperData MapperData { get; internal set; }

        public Type GetSourceMemberRuntimeType(IQualifiedMember sourceMember)
        {
            if (_parent.Source == null)
            {
                return sourceMember.Type;
            }

            if (sourceMember.Type.IsSealed())
            {
                return sourceMember.Type;
            }

            if (sourceMember == MapperData.SourceMember)
            {
                return sourceMember.Type;
            }

            var getRuntimeTypeFunc = _runtimeTypeGettersCache.GetOrAdd(sourceMember, sm =>
            {
                var sourceParameter = Parameters.Create<TSource>("source");
                var relativeMember = sm.RelativeTo(MapperData.SourceMember);
                var memberAccess = relativeMember.GetQualifiedAccess(MapperData.SourceObject);
                memberAccess = memberAccess.Replace(MapperData.SourceObject, sourceParameter);

                var getRuntimeTypeCall = Expression.Call(
                    ObjectExtensions.GetRuntimeSourceTypeMethod.MakeGenericMethod(sm.Type),
                    memberAccess);

                var getRuntimeTypeLambda = Expression
                    .Lambda<Func<TSource, Type>>(getRuntimeTypeCall, sourceParameter);

                return getRuntimeTypeLambda.Compile();
            });

            return getRuntimeTypeFunc.Invoke(_parent.Source);
        }
    }
}