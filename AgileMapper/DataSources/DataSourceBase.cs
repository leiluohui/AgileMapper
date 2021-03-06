﻿namespace AgileObjects.AgileMapper.DataSources
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Extensions;
    using Members;

    internal abstract class DataSourceBase : IDataSource
    {
        protected DataSourceBase(IQualifiedMember sourceMember, Expression value)
            : this(sourceMember, Enumerable<ParameterExpression>.EmptyArray, value)
        {
        }

        protected DataSourceBase(IDataSource wrappedDataSource, Expression value = null)
            : this(
                  wrappedDataSource.SourceMember,
                  wrappedDataSource.Variables,
                  value ?? wrappedDataSource.Value,
                  wrappedDataSource.Condition)
        {
        }

        protected DataSourceBase(
            IQualifiedMember sourceMember,
            ICollection<ParameterExpression> variables,
            Expression value,
            Expression condition = null)
        {
            SourceMember = sourceMember;
            Condition = condition;
            Variables = variables;
            Value = value;
        }

        protected DataSourceBase(
            IQualifiedMember sourceMember,
            Expression value,
            IMemberMapperData mapperData)
        {
            SourceMember = sourceMember;

            Expression[] nestedAccesses;
            ICollection<ParameterExpression> variables;

            ProcessNestedAccesses(
                mapperData,
                ref value,
                out nestedAccesses,
                out variables);

            Condition = nestedAccesses.GetIsNotDefaultComparisonsOrNull();
            Variables = variables;
            Value = value;
        }

        #region Setup

        private static void ProcessNestedAccesses(
            IMemberMapperData mapperData,
            ref Expression value,
            out Expression[] nestedAccesses,
            out ICollection<ParameterExpression> variables)
        {
            nestedAccesses = mapperData.GetNestedAccessesIn(value, targetCanBeNull: false);
            variables = new List<ParameterExpression>();

            if (nestedAccesses.None())
            {
                return;
            }

            var nestedAccessVariableByNestedAccess = new Dictionary<Expression, Expression>();

            for (var i = 0; i < nestedAccesses.Length; i++)
            {
                var nestedAccess = nestedAccesses[i];

                if (CacheValueInVariable(nestedAccess))
                {
                    var valueVariable = Expression.Variable(nestedAccess.Type, "accessValue");
                    nestedAccesses[i] = valueVariable.AssignTo(nestedAccess);

                    nestedAccessVariableByNestedAccess.Add(nestedAccess, valueVariable);
                    variables.Add(valueVariable);
                }
            }

            value = value.Replace(nestedAccessVariableByNestedAccess);
        }

        private static bool CacheValueInVariable(Expression value)
            => (value.NodeType == ExpressionType.Call) || (value.NodeType == ExpressionType.Invoke);

        #endregion

        public IQualifiedMember SourceMember { get; }

        public Expression SourceMemberTypeTest { get; protected set; }

        public virtual bool IsValid => Value != Constants.EmptyExpression;

        public virtual Expression PreCondition => null;

        public bool IsConditional => Condition != null;

        public virtual Expression Condition { get; }

        public ICollection<ParameterExpression> Variables { get; }

        public Expression Value { get; }

        public virtual Expression AddPreCondition(Expression population) => population;

        public Expression AddCondition(Expression value, Expression alternateBranch = null)
        {
            return alternateBranch != null
                ? Expression.IfThenElse(Condition, value, alternateBranch)
                : Expression.IfThen(Condition, value);
        }

        public Expression GetTargetMemberPopulation(IMemberMapperData mapperData)
            => mapperData.GetTargetMemberPopulation(Value);
    }
}