﻿using AutoMapper;
using Microsoft.AspNet.Identity;
using MyShop.Model.Models;
using MyShop.Service;
using MyShop.WebAPI.Infrastructure.Core;
using MyShop.WebAPI.Infrastructure.Extentions;
using MyShop.WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyShop.WebAPI.Controllers
{
    //[Authorize]
    [RoutePrefix("api/function")]
    public class FunctionController : ApiControllerBase
    {
        private IFunctionService _functionService;

        public FunctionController(IErrorService errorService, IFunctionService functionService) : base(errorService)
        {
            _functionService = functionService;
        }

        [Route("getlisthierarchy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierarchy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IEnumerable<Function> model;
                if (User.IsInRole("Admin"))
                {
                    model = _functionService.GetAll(string.Empty);
                }
                else
                {
                    model = _functionService.GetAllWithPermission(User.Identity.GetUserId());
                }

                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                var parents = modelVm.Where(x => x.Parent == null);
                foreach (var parent in parents)
                {
                    parent.ChildFunctions = modelVm.Where(x => x.ParentId == parent.ID).ToList();
                }
                response = request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(parents, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string filter = "")
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _functionService.GetAll(filter);
                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("detail/id")]
        [HttpGet]
        public HttpResponseMessage detail(HttpRequestMessage request, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " Không có giá trị.");
            }
            var function = _functionService.Get(id);
            if (function == null)
            {
                return request.CreateErrorResponse(HttpStatusCode.NoContent, "No data");
            }
            var modelVM = Mapper.Map<Function, FunctionViewModel>(function);
            return request.CreateResponse(HttpStatusCode.OK, modelVM);
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var newFunction = new Function();
                try
                {
                    if (_functionService.CheckExistedId(functionViewModel.ID))
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID đã tồn tại");
                    }
                    else
                    {
                        newFunction.UpdateFunction(functionViewModel);
                        _functionService.Create(newFunction);
                        _functionService.Save();
                        return request.CreateResponse(HttpStatusCode.OK, functionViewModel);
                    }
                }
                catch (Exception dex)
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
        public HttpResponseMessage Update(HttpRequestMessage request, FunctionViewModel functionViewModel)
        {
            if (ModelState.IsValid)
            {
                var function = _functionService.Get(functionViewModel.ID);
                try
                {
                    if (functionViewModel.ParentId == "") functionViewModel.ParentId = null;
                    function.UpdateFunction(functionViewModel);
                    _functionService.Update(function);
                    _functionService.Save();

                    return request.CreateResponse(HttpStatusCode.OK, function);
                }
                catch (Exception dex)
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
            _functionService.Delete(id);
            _functionService.Save();

            return request.CreateResponse(HttpStatusCode.OK, id);
        }
    }
}
