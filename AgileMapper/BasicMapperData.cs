namespace AgileObjects.AgileMapper
{
    using System;
    using Configuration;
    using Members;

    internal class BasicMapperData : IBasicMapperData
    {
        public BasicMapperData(
            MappingRuleSet ruleSet,
            Type sourceType,
            Type targetType,
            QualifiedMember targetMember = null,
            IBasicMapperData parent = null)
        {
            SourceType = sourceType;
            TargetType = targetType;
            RuleSet = ruleSet;
            TargetMember = targetMember ?? QualifiedMember.All;
            Parent = parent;
        }

        public static BasicMapperData WithNoTargetMember(MemberMapperData parent)
        {
            return new BasicMapperData(
                parent.RuleSet,
                parent.SourceType,
                parent.TargetMember.Type,
                QualifiedMember.None,
                parent);
        }

        public IBasicMapperData Parent { get; }

        public MappingRuleSet RuleSet { get; }

        public Type SourceType { get; }

        public Type TargetType { get; }

        public QualifiedMember TargetMember { get; }
    }
}