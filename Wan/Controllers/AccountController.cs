﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using FormBuilder.Business.Entities;
using FormBuilder.Data.Contracts;
using Microsoft.Web.WebPages.OAuth;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Wan.Controllers.ApiControllers;
using Wan.Services;
using WebMatrix.WebData;
using Wan.Models;

namespace Wan.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private IApplicationUnit _applicationUnit;
        private ModelFactoryService _modelFactory;

        public AccountController(IApplicationUnit applicationUnit)
        {
            _applicationUnit = applicationUnit;
            _modelFactory = new ModelFactoryService();
        }
        [Authorize]
        public JsonResult JoinGroupRequest()
        {
            var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

            var groupsWithinManagerRole = currentUser.Groups.Where(m => m.UserGroupRoles.Select(ugr => ugr.UserId).Contains(currentUser.Id)).Select(g => g.Id);

            var joinGroupRequestMessages =
                _applicationUnit.JoinGroupRequestRepository.Get(
                    m => !m.IsProcessed & groupsWithinManagerRole.Contains(m.GroupId), null, "User").Select(jgr => _modelFactory.Create(jgr)).ToList();

            return Json(new {messages = joinGroupRequestMessages}, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginAjax(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return Json(new { status = true, userName = model.UserName });
            }
            return Json(new {status = false, error = "Error please try again."});
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }


        public ActionResult LogOut()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        ForceChangePassword = false,
                        CreatedDate = DateTime.Now
                    });
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterAjax(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        ForceChangePassword = false,
                        CreatedDate = DateTime.Now
                    });
                    WebSecurity.Login(model.UserName, model.Password);
                    return Json(new { status = true, userName = model.UserName });
                }
                catch (MembershipCreateUserException e)
                {                    
                    return Json(new { status = false, userName = model.UserName, message = ErrorCodeToString(e.StatusCode) });
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(new { status = false, userName = model.UserName, message = "Please try again later." });
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            var changePasswordSucceeded = false;
            
                if (ModelState.IsValid)
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword,
                                                                         model.NewPassword);

                    if (changePasswordSucceeded)
                    {
                        return Json(new {succeeded = changePasswordSucceeded});
                    }
                    else
                    {
                        return Json(new { succeeded = changePasswordSucceeded, message = "Current password is wrong, or new password is invalid." });
                    }
                    
                }

            return Json(new {succeeded = false});

        }

        [HttpPost]
        [Authorize]
        public ActionResult SaveMyDetails(MyDetailsModel model)
        {

            try
            {
                User currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
                currentUser.DOB = model.DOB;
                currentUser.City = model.City;
                currentUser.NickName = model.NickName;
                currentUser.AboutMe = model.AboutMe;

                _applicationUnit.UserRepository.Update(currentUser);
                _applicationUnit.SaveChanges();
                return Json(new { succeeded = true });
            }
            catch (Exception ex)
            {
                //Todo: log ex;
                return Json(new { succeeded = false, message = "Error, Please try again later" });
            }

            return Json(new { succeeded = false });

        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadImage()
        {            
            var request = Request;

            try
            {
                var uploadedFile = Request.Files["uploadProfilePic"];
                var xFileName = request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;

                var fileValidationService = new FileValidationService();
                if (fileValidationService.ValidateUpload(fileName) && request.ContentLength < 550000)
                {
                    var contentType = request.ContentType;
                    var contentLength = request.ContentLength;

                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);
                    var urlstring = blob.Uri.ToString();

                    var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

                    if (currentUser.ProfileImage != null)
                    {
                        var imagesContainer = blobStorage.GetContainerReference("productimages");
                        var oldImageToDelete = imagesContainer.GetBlockBlobReference(currentUser.ProfileImage);
                        oldImageToDelete.DeleteIfExists();
                    }                    
                    currentUser.ProfileImage = urlstring;
                    currentUser.ContentType = contentType;

                    _applicationUnit.UserRepository.Update(currentUser);
                    _applicationUnit.SaveChanges();
                    return Json(new { succeeded = true, imageFile = urlstring });  
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return Json(new { succeeded = false });  
            }

            return Json(new { succeeded = false });
        }


        [HttpPost]
        [Authorize]
        public ActionResult UploadGroupImage()
        {
            //var request = HttpContext.Current.Request;

            try
            {
                var uploadedFile = Request.Files["uploadProfilePic"];
                var xFileName = Request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;
                int groupId;
                int.TryParse(Request.Form["groupId"], out groupId);
                var group = _applicationUnit.GroupRepository.GetByID(groupId);
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
                var contentType = Request.ContentType;

                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && Request.ContentLength < 550000 && groupSecService.IsUserGroupManager(currentUser, group.UserGroupRoles.ToList()))
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);


                    var urlstring = blob.Uri.ToString();

                    if (group.GroupImage != null)
                    {
                        var imagesContainer = blobStorage.GetContainerReference("productimages");
                        var oldImageToDelete = imagesContainer.GetBlockBlobReference(group.GroupImage);
                        oldImageToDelete.DeleteIfExists();
                    }

                    group.GroupImage = urlstring;
                    currentUser.ContentType = contentType;

                    _applicationUnit.GroupRepository.Update(group);
                    _applicationUnit.SaveChanges();

                    return Json(new { succeeded = true, imageFile = urlstring });
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return Json(new { succeeded = false});
            }

             return Json(new { succeeded = false});
        }


        [HttpPost]
        [Authorize]
        public ActionResult UploadSponsorImage()
        {
            try
            {
                var uploadedFile = Request.Files["uploadProfilePic"];
                var xFileName = Request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;
                int sponsorId;
                int.TryParse(Request.Form["sponsorId"], out sponsorId);
                var sponsor = _applicationUnit.SponsoRepository.Get(m => m.Id == sponsorId, null, "Group").First();
                var group = sponsor.Group;
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
                var contentType = Request.ContentType;
                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && Request.ContentLength < 550000 && groupSecService.IsUserGroupManager(currentUser, group.UserGroupRoles.ToList()))
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);


                    var urlstring = blob.Uri.ToString();

                    if (sponsor.PhotoUrl != null)
                    {
                        var imagesContainer = blobStorage.GetContainerReference("productimages");
                        var oldImageToDelete = imagesContainer.GetBlockBlobReference(sponsor.PhotoUrl);
                        oldImageToDelete.DeleteIfExists();
                    }

                    sponsor.PhotoUrl = urlstring;
                    currentUser.ContentType = contentType;

                    _applicationUnit.SponsoRepository.Update(sponsor);
                    _applicationUnit.SaveChanges();

                    return Json(new { succeeded = true, imageFile = urlstring });
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return Json(new { succeeded = false });
            }

            return Json(new { succeeded = false });
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadGroupPhoto()
        {
            try
            {
                var uploadedFile = Request.Files["uploadInputElement"];
                var xFileName = Request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;
                int groupId;
                int.TryParse(Request.Form["groupId"], out groupId);
                var group = _applicationUnit.GroupRepository.Get(m => m.Id == groupId, null, "UserGroupRoles").First();
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
                var contentType = Request.ContentType;
                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && Request.ContentLength < 550000 && groupSecService.IsUserGroupManager(currentUser, group.UserGroupRoles.ToList()))
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);


                    var urlstring = blob.Uri.ToString();

                    var newGroupPhoto = new GroupPhoto()
                    {
                        GroupId = groupId,
                        Url = urlstring
                    };

                    group.GroupPhotos.Add(newGroupPhoto);


                    _applicationUnit.GroupRepository.Update(group);
                    _applicationUnit.SaveChanges();

                    return Json(new { succeeded = true, groupPhoto = new GroupPhotoViewModel{ Id = newGroupPhoto.Id, GroupId = groupId, Url = newGroupPhoto.Url} });
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return Json(new { succeeded = false });
            }

            return Json(new { succeeded = false });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UploadEventAd()
        {
            try
            {
                var uploadedFile = Request.Files["uploadInputElement"];
                var xFileName = Request.Headers["X-File-Name"];
                var formFilename = uploadedFile.FileName;
                Stream inputStream = uploadedFile.InputStream;
                var fileName = xFileName ?? formFilename;
                int eventId;
                int.TryParse(Request.Form["eventId"], out eventId);

                string adSiteUrl = Request.Form["adSiteUrl"];

                var eventDomain = _applicationUnit.EventRepository.GetByID(eventId);
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);
                var contentType = Request.ContentType;
                var fileValidationService = new FileValidationService();
                var groupSecService = new GroupSecurityService();

                if (fileValidationService.ValidateUpload(fileName) && Request.ContentLength < 550000)
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                    var blobStorage = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobStorage.GetContainerReference("productimages");
                    if (container.CreateIfNotExist())
                    {
                        // configure container for public access
                        var permissions = container.GetPermissions();
                        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                        container.SetPermissions(permissions);
                    }

                    string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                                                                Guid.NewGuid().ToString(), Path.GetExtension(uploadedFile.FileName));
                    CloudBlockBlob blob = blobStorage.GetBlockBlobReference(uniqueBlobName);
                    blob.Properties.ContentType = uploadedFile.ContentType;
                    blob.UploadFromStream(uploadedFile.InputStream);


                    var urlstring = blob.Uri.ToString();

                    if (eventDomain.AdUrl != null)
                    {
                        var imagesContainer = blobStorage.GetContainerReference("productimages");
                        var oldImageToDelete = imagesContainer.GetBlockBlobReference(eventDomain.AdUrl);
                        oldImageToDelete.DeleteIfExists();
                    }

                    eventDomain.AdUrl = urlstring;
                    eventDomain.AdSiteUrl = adSiteUrl;
                   
                    _applicationUnit.EventRepository.Update(eventDomain);
                    _applicationUnit.SaveChanges();

                    return Json(new { succeeded = true, url = urlstring});
                }
            }
            catch (Exception ex)
            {
                //log error todo
                return Json(new { succeeded = false });
            }

            return Json(new { succeeded = false });
        }


        [AllowAnonymous]
        public JsonResult GetUserImage()
        {
            if (WebSecurity.IsAuthenticated)
            {
                var currentUser = _applicationUnit.UserRepository.GetByID(WebSecurity.CurrentUserId);

                return Json(new { image = currentUser.ProfileImage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }



        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                //using (UsersContext db = new UsersContext())
                //{
                //    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                //    // Check if user already exists
                //    if (user == null)
                //    {
                //        // Insert name into the profile table
                //        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                //        db.SaveChanges();

                //        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                //        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                //        return RedirectToLocal(returnUrl);
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                //    }
                //}
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
