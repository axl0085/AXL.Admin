using AXL.Model;
using AXL.Model.System;
using AXL.Model.System.Dto;
using System.Collections.Generic;

namespace AXL.Service.System.IService {

    public interface IArticleCategoryService : IBaseService<ArticleCategory> {

        PagedInfo<ArticleCategory> GetList(ArticleCategoryQueryDto parm);

        List<ArticleCategory> GetTreeList(ArticleCategoryQueryDto parm);

        int AddArticleCategory(ArticleCategory parm);
    }
}