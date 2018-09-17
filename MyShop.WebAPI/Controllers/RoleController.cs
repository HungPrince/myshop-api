using AutoMapper;
using Microsoft.AspNet.Identity;
using MyShop.Common.Exceptions;
using MyShop.Model.Models;
using MyShop.Service;
using MyShop.WebAPI.Infrastructure.Core;
using MyShop.WebAPI.Infrastructure.Extentions;
using MyShop.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace MyShop.WebAPI.Controllers
{
    [RoutePrefix("api/role")]
    public class RoleController : ApiControllerBase
    {
        private IPermissionService _permissionService;
        private IFunctionService _functionService;

        public RoleController(IErrorService errorService, IFunctionService functionService, IPermissionService permissionService) : base(errorService)
        {
            _functionService = functionService;
            _permissionService = permissionService;
        }

        [Route("getlistpaging")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var query = AppRoleManager.Roles;
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(x => x.Description.Contains(filter));
                }
                totalRow = query.Count();
                var model = query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize);
                IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);

                PaginationSet<ApplicationRoleViewModel> pagedSet = new PaginationSet<ApplicationRoleViewModel>()
                {
                    PageIndex = page,
                    TotalRows = totalRow,
                    PageSize = pageSize,
                    Items = modelVm
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, ApplicationRoleViewModel applicationRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var newRole = new AppRole();
                newRole.UpdateApplicationRole(applicationRoleViewModel);
                try
                {
                    AppRoleManager.Create(newRole);
                    return request.CreateResponse(HttpStatusCode.OK, applicationRoleViewModel);
                }
                catch (NameDuplicatedException dex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, dex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}