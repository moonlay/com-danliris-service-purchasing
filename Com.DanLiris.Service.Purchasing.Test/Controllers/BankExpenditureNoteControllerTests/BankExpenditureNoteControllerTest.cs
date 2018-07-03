﻿using AutoMapper;
using Com.DanLiris.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.DanLiris.Service.Purchasing.Lib.Helpers.ReadResponse;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.DanLiris.Service.Purchasing.Lib.Services;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.BankExpenditureNote;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.DanLiris.Service.Purchasing.Test.Helpers;
using Com.DanLiris.Service.Purchasing.WebApi.Controllers.v1.BankExpenditureNote;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Purchasing.Test.Controllers.BankExpenditureNoteControllerTests
{
    public class BankExpenditureNoteControllerTest
    {
        private BankExpenditureNoteViewModel ViewModel
        {
            get
            {
                return new BankExpenditureNoteViewModel()
                {
                    Bank = new AccountBankViewModel() { currency = new CurrencyViewModel() },
                    Details = new List<BankExpenditureNoteDetailViewModel>() { new BankExpenditureNoteDetailViewModel() { Items = new List<BankExpenditureNoteItemViewModel>() { new BankExpenditureNoteItemViewModel() } } }
                };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private BankExpenditureNoteController GetController(Mock<IBankExpenditureNoteFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            servicePMock
                .Setup(x => x.GetService(typeof(IValidateService)))
                .Returns(validateM.Object);

            BankExpenditureNoteController controller = new BankExpenditureNoteController(servicePMock.Object, facadeM.Object, mapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_All_Expedition_Data()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Read(1, 25, "{}", null, "{}"))
                .Returns(new ReadResponse(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.Get(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_By_Position_Data()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.GetAllByPosition(1, 25, "{}", null, "{}"))
                .Returns(new ReadResponse(new List<object>(), 1, new Dictionary<string, string>()));
            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetAllCashierPosition(1, 25, "{}", null, "{}");
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync(Model);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(s => s.Map<BankExpenditureNoteViewModel>(It.IsAny<BankExpenditureNoteModel>()))
                .Returns(new BankExpenditureNoteViewModel());

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        private BankExpenditureNoteModel Model
        {
            get
            {
                return new BankExpenditureNoteModel()
                {
                    Active = true,
                    BankAccountName = "",
                    BankAccountNumber = "",
                    BankCode = "",
                    BankId = "",
                    BankName = "",
                    BGCheckNumber = "",
                    CreatedAgent = "",
                    CreatedBy = "",
                    CreatedUtc = DateTime.UtcNow,
                    BankCurrencyCode = "",
                    BankCurrencyId = "",
                    BankCurrencyRate = "",
                    DeletedAgent = "",
                    DeletedBy = "",
                    DeletedUtc = DateTime.UtcNow,
                    Id = 1,
                    IsDeleted = false,
                    Details = new List<BankExpenditureNoteDetailModel>() { new BankExpenditureNoteDetailModel() { Items = new List<BankExpenditureNoteItemModel>() { new BankExpenditureNoteItemModel() } } },
                };
            }
        }

        [Fact]
        public void Should_Not_Found_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .ReturnsAsync((BankExpenditureNoteModel)null);

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
               .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            BankExpenditureNoteController controller = new BankExpenditureNoteController(GetServiceProvider().Object, mockFacade.Object, mockMapper.Object);
            var response = controller.GetById(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Post(this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_Bad_Request_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Post(this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Post(this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Put(0, this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_Bad_Request_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Put(0, ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_Bad_Request_Id_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Throws(GetServiceValidationExeption());

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(s => s.Map<BankExpenditureNoteModel>(It.IsAny<BankExpenditureNoteViewModel>()))
                .Returns(new BankExpenditureNoteModel());

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Put(1, ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Update_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<BankExpenditureNoteModel>(), "unittestusername"))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Put(0, this.ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Return_Not_Found_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(0);

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.NotFound, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<BankExpenditureNoteViewModel>())).Verifiable();

            var mockFacade = new Mock<IBankExpenditureNoteFacade>();
            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
               .ThrowsAsync(new Exception());

            var mockMapper = new Mock<IMapper>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = controller.Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}