namespace AgileObjects.AgileMapper.ObjectPopulation
{
    using System.Linq.Expressions;
    using DataSources;
    using Extensions;
    using Members;

    internal class ExistingOrDefaultValueDataSourceFactory : IDataSourceFactory
    {
        public static readonly IDataSourceFactory Instance = new ExistingOrDefaultValueDataSourceFactory();

        public IDataSource Create(IMemberMapperData mapperData)
            => new ExistingMemberValueOrEmptyDataSource(mapperData);

        private class ExistingMemberValueOrEmptyDataSource : DataSourceBase
        {
            public ExistingMemberValueOrEmptyDataSource(IMemberMapperData mapperData)
                : base(mapperData.SourceMember, GetValue(mapperData), mapperData)
            {
            }

            private static Expression GetValue(IMemberMapperData mapperData)
            {
                if (mapperData.TargetMember.IsEnumerable)
                {
                    return FallbackToNull(mapperData)
                        ? mapperData.TargetMember.Type.ToDefaultExpression()
                        : mapperData.GetFallbackCollectionValue();
                }

                if (mapperData.TargetMember.IsReadable)
                {
                    return mapperData.GetTargetMemberAccess();
                }

                return mapperData.TargetMember.Type.ToDefaultExpression();
            }

            private static bool FallbackToNull(IBasicMapperData mapperData)
            {
                if (mapperData.TargetMember.IsDictionary)
                {
                    return false;
                }

                var dictionaryTargetMember = mapperData.TargetMember as DictionaryTargetMember;

                if (dictionaryTargetMember == null)
                {
                    return false;
                }

                if (dictionaryTargetMember.HasEnumerableEntries)
                {
                    return false;
                }

                return true;
            }
        }
    }
}