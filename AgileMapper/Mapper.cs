﻿namespace AgileObjects.AgileMapper
{
    using Api;
    using Api.Configuration;

    public sealed class Mapper : IMapper
    {
        private static readonly IMapper _default = Create();

        private readonly MapperContext _mapperContext;

        private Mapper(MapperContext mapperContext)
        {
            _mapperContext = mapperContext;
        }

        #region Factory Methods

        public static IMapper Create()
        {
            return new Mapper(new MapperContext());
        }

        #endregion

        PlanTargetTypeSelector<TSource> IMapper.GetPlanFor<TSource>()
            => new PlanTargetTypeSelector<TSource>(_mapperContext);

        PreEventConfigStartingPoint IMapper.Before => new PreEventConfigStartingPoint(_mapperContext);

        PostEventConfigStartingPoint IMapper.After => new PostEventConfigStartingPoint(_mapperContext);

        #region Static Access Methods

        public static PlanTargetTypeSelector<TSource> GetPlanFor<TSource>() => _default.GetPlanFor<TSource>();

        public static PreEventConfigStartingPoint Before => _default.Before;

        public static PostEventConfigStartingPoint After => _default.After;

        public static MappingConfigStartingPoint WhenMapping => _default.WhenMapping;

        public static TSource Clone<TSource>(TSource source) where TSource : class
            => _default.Clone(source);

        public static TargetTypeSelector<TSource> Map<TSource>(TSource source) => _default.Map(source);

        internal static void ResetDefaultInstance() => _default.Dispose();

        #endregion

        MappingConfigStartingPoint IMapper.WhenMapping => new MappingConfigStartingPoint(_mapperContext);

        TSource IMapper.Clone<TSource>(TSource source) => Map(source).ToNew<TSource>();

        TargetTypeSelector<TSource> IMapper.Map<TSource>(TSource source)
        {
            return new TargetTypeSelector<TSource>(source, _mapperContext);
        }

        #region IDisposable Members

        public void Dispose() => _mapperContext.Reset();

        #endregion
    }
}
