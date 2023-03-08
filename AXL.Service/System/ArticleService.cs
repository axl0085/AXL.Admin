using AXL.Infrastructure.Attribute;
using AXL.Model.System;
using AXL.Service.System.IService;

namespace AXL.Service.System {

    /// <summary>
    ///
    /// </summary>
    [AppService(ServiceType = typeof(IArticleService), ServiceLifetime = LifeTime.Transient)]
    public class ArticleService : BaseService<Article>, IArticleService {
    }
}