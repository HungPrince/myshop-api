using AutoMapper;
using Microsoft.AspNet.Identity;
using MyShop.Common.Exceptions;
using MyShop.Model.Models;
using MyShop.Service;
using MyShop.WebAPI.Infrastructure.Core;
using MyShop.WebAPI.Infrastructure.Extentions;
using MyShop.WebAPI.Models;
using MyShop.WebAPI.Models.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

        [HttpGet]
        [Route("getlistpaging")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var query = AppRoleManager.Roles;
                if (page == 0)
                {
                    var model = query.OrderBy(x => x.Name);
                    IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);
                    response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                }
                else
                {
                    int totalRow = 0;
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
                }

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
                    return request.CreateResponse(HttpStatusCode.OK, newRole);
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

        [HttpPut]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationRoleViewModel applicationRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var appRole = AppRoleManager.FindById(applicationRoleViewModel.Id);
                try
                {
                    appRole.UpdateApplicationRole(applicationRoleViewModel, "update");
                    AppRoleManager.Update(appRole);
                    return request.CreateResponse(HttpStatusCode.OK, appRole);
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

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            var role = AppRoleManager.FindById(id);
            AppRoleManager.Delete(role);
            return request.CreateResponse(HttpStatusCode.OK, id);
        }

        [HttpGet]
        [Route("getAllPermission")]
        public HttpResponseMessage GetAllPermission(HttpRequestMessage request, string functionId)
        {
            return CreateHttpResponse(request, () =>
            {
                List<PermissionViewModel> permissions = new List<PermissionViewModel>();
                HttpResponseMessage response = null;
                var roles = AppRoleManager.Roles.Where(x => x.Name != "Admin").ToList();
                var listPermission = _permissionService.GetByFunctionId(functionId).ToList();
                if (listPermission.Count == 0)
                {
                    foreach (var item in roles)
                    {
                        permissions.Add(new PermissionViewModel()
                        {
                            RoleId = item.Id,
                            CanCreate = false,
                            CanUpdate = false,
                            CanRead = false,
                            CanDelete = false,
                            AppRole = new ApplicationRoleViewModel()
                            {
                                Id = item.Id,
                                Description = item.Description,
                                Name = item.Name
                            }
                        });
                    }
                }
                else
                {
                    foreach (var item in roles)
                    {
                        if(!listPermission.Any(x=>x.RoleId == item.Id))
                        {
                            permissions.Add(new PermissionViewModel()
                            {
                                RoleId = item.Id,
                                CanCreate = false,
                                CanUpdate = false,
                                CanRead = false,
                                CanDelete = false,
                                AppRole = new ApplicationRoleViewModel()
                                {
                                    Id = item.Id,
                                    Description = item.Description,
                                    Name = item.Name
                                }
                            });
                        }
                        permissions = Mapper.Map<List<Permission>, List<PermissionViewModel>>(listPermission);
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, permissions);
                return response;
            });
            
        }

        [HttpPost]
        [Route("savePermission")]
        public HttpResponseMessage SavePermisssion(HttpRequestMessage request, SavePermissionRequest data)
        {
            if (ModelState.IsValid)
            {
                _permissionService.DeleteAll(data.FunctionId);
                Permission permission = null;
                foreach (var item in data.Permissions)
                {
                    permission = new Permission();
                    permission.UpdatePermission(item);
                    permission.FunctionId = data.FunctionId;
                    _permissionService.Add(permission);
                }
                var functions = _functionService.GetAllWithParentID(data.FunctionId);
                if (functions.Any())
                {
                    foreach (var item in functions)
                    {
                        _permissionService.DeleteAll(item.ID);
                        foreach (var p in data.Permissions)
                        {
                            var childPermission = new Permission();
                            childPermission.FunctionId = item.ID;
                            childPermission.RoleId = p.RoleId;
                            childPermission.CanRead = p.CanRead;
                            childPermission.CanUpdate = p.CanUpdate;
                            childPermission.CanDelete = p.CanDelete;
                            childPermission.CanCreate = p.CanCreate;
                            _permissionService.Add(childPermission);
                        }
                    }
                }
                try
                {
                    _permissionService.SaveChange();
                    return request.CreateResponse(HttpStatusCode.OK, "Lưu quyền thành công");
                }
                catch(Exception ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}