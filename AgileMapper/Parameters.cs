namespace AgileObjects.AgileMapper
{
    using System;
    using System.Linq.Expressions;
    using Extensions;
    using Members;
    using ObjectPopulation;

    internal static class Parameters
    {
        public static readonly ParameterExpression MappingContext = Create<MappingContext>();
        public static readonly ParameterExpression MappingData = Create<IMappingData>();
        public static readonly ParameterExpression ObjectMapperData = Create<ObjectMapperData>();
        public static readonly ParameterExpression ObjectMappingContextData = Create<IObjectMappingContextData>();

        public static readonly ParameterExpression EnumerableIndex = Create<int>("i");
        public static readonly ParameterExpression EnumerableIndexNullable = Create<int?>("i");

        public static readonly ParameterExpression SourceMember = Create<IQualifiedMember>("sourceMember");
        public static readonly ParameterExpression TargetMember = Create<QualifiedMember>("targetMember");
        public static readonly ParameterExpression RuntimeTypesAreTheSame = Create<bool>("runtimeTypesAreTheSame");

        public static ParameterExpression Create<T>(string name = null) => Create(typeof(T), name);

        public static ParameterExpression Create(Type type) => Create(type, type.GetShortVariableName());

        public static ParameterExpression Create(Type type, string name)
            => Expression.Parameter(type, name ?? type.GetShortVariableName());
    }
}