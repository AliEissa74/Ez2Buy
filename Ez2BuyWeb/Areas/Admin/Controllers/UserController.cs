using Ez2Buy.DataAccess.Contracts;
using Ez2Buy.DataAccess.Data;
using Ez2Buy.DataAccess.Models;
using Ez2Buy.Services.Contracts;
using Ez2Buy.Services.Services;
using Ez2Buy.Utility;
using Ez2BuyWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Ez2BuyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
		// we use dbcontext to access the database tables like roles
		private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
			_userManager = userManager;
		}
        public IActionResult Index() 
        {
           
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId; //get the role id of the user
            RoleManagementVM RoleVM = new ()
            {
                AppUser = _db.AppUsers.FirstOrDefault(u => u.Id == userId), //get the user by id
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            RoleVM.AppUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name; //get the role name of the user
            return View(RoleVM);
        }

        [HttpPost]
		public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
		{
			string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.AppUser.Id).RoleId; //get the role id of the user
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name; //get the old role name of the user
            if (!(roleManagementVM.AppUser.Role == oldRole))
            {
                //a role was updated
                AppUser appUser = _db.AppUsers.FirstOrDefault(u => u.Id == roleManagementVM.AppUser.Id);
                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(appUser, oldRole).GetAwaiter().GetResult(); //remove the user from the old role
				_userManager.AddToRoleAsync(appUser, roleManagementVM.AppUser.Role).GetAwaiter().GetResult(); //remove the user from the old role
			}
			return RedirectToAction("Index");
		}



		#region API CALLS

		[HttpGet]
        public IActionResult GetAll()
        {

            List<AppUser> objUserList = _db.AppUsers.ToList();  //get all users
			var userRoles = _db.UserRoles.ToList();             //get all user roles
			var roles = _db.Roles.ToList();                     //get all roles
			foreach ( var user in objUserList)
            {
				//want to access roles table
				var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId; //we get the role id of the user
                user.Role= roles.FirstOrDefault(u => u.Id == roleId).Name; //we get the role name of the user
			}
            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id) //from body specifies that parameter is coming from the body of the request(ajax call)
        {
            //base on id we get the user records
            var objFromDb = _db.AppUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
			string message = "";
			//check if the user is locked or not
			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked we need to unlock him
                objFromDb.LockoutEnd = DateTime.Now; //unlock the user
                message = "User account unlocked successfully";
			}
			else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(500); //lock the user for 500 year
				message = "User account locked successfully";
			}
            _db.SaveChanges();
            return Json(new { success = true, message = message });
        }

        #endregion
    }
}
