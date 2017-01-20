//
//  MerchantController.cs
//  merchant_integration
//  
//  Created by Juan Pablo Yiguerimián on 10/25/16.
//  Copyright © 2016 ForceITS. All rights reserved.
//
//  The software code, programs, and documentation are the confidential and
//  proprietary information of ForceITS Corporation ("Confidential Information").
//  You shall not disclose such Confidential Information and shall use it only
//  in accordance with the terms of the license agreement you entered into
//  with ForceITS or one of its licensed distributors.
//
//  ForceITS MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
//  SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//  PURPOSE, OR NON-INFRINGEMENT. ForceITS SHALL NOT BE LIABLE FOR ANY DAMAGES
//  SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
//  THIS SOFTWARE OR ITS DERIVATIVES.
//

using System;
using Microsoft.AspNetCore.Mvc;
using merchant_integration.Core;

namespace merchant_integration.Controllers
{

    /// <summary>
    /// This class is a controller that processes requests about Merchants, for example, 'Payment'.
    /// </summary>
    public class MerchantController : Controller
    {

        /// <summary>
        /// This is the method that set the correct parameters to show a view to do the payment
        /// </summary>
        /// <returns>A Payment View</returns>
        public IActionResult Payment()
        {
            try
            {
                SetBasicParameters();
                SetPaymentParameters();
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Description"] = ex.Message;
                return View("Error");
            }
        }

        /// <summary>
        /// This method calls the API set in configuration to get a token that will be used to do a payment
        /// </summary>
        /// <returns>A Payment View</returns>
        public IActionResult GenerateAuth()
        {
            try
            {
                SetBasicParameters();
                ViewData["token"] = (new Authentication()).GetToken(Request.Form["merchantId"], Request.Form["apiKey"]);
                SetPaymentParametersFromForm();
                return View("Payment");
            }
            catch (Exception ex)
            {
                ViewData["Description"] = ex.Message;
                return View("Error");
            }
        }

        /// <summary>
        /// This method sends basic parameters to the view, such as title, API endpoint, library paths.
        /// The will be used to show messages in the view and process the payment action in javascript.
        /// </summary>
        private void SetBasicParameters()
        {
            ViewData["Title"] = "Merchant Integration for ASP.NET Core";
            ViewData["Message"] = "Payment";
            ViewData["EndPoint"] = string.Format("{0}{1}:{2}", "http://", Program.Configuration["UIApp:Host"], Program.Configuration["UIApp:Port"]);
            ViewData["MainJsLibraryPath"] = string.Format("{0}{1}:{2}/{3}", "http://", Program.Configuration["UIApp:Host"], Program.Configuration["UIApp:Port"], Program.Configuration["UIApp:MainJsLibraryPath"]);
            ViewData["MainStyleSheetPath"] = string.Format("{0}{1}:{2}/{3}", "http://", Program.Configuration["UIApp:Host"], Program.Configuration["UIApp:Port"], Program.Configuration["UIApp:MainStyleSheetPath"]);
        }

        /// <summary>
        /// This method initialize the Payment View with some parameters such as merchant, currency, amount.
        /// </summary>
        private void SetPaymentParameters()
        {
            ViewData["merchantId"] = Program.Configuration["MerchantLogin:MerchantId"];
            ViewData["apikey"] = Program.Configuration["MerchantLogin:ApiKey"];
            ViewData["pdid"] = "";
            ViewData["currency"] = "CLP";
            ViewData["amount"] = "6534";
            ViewData["detail"] = "Reservation #123";
            ViewData["type"] = "online";
            ViewData["country"] = "CHL";
            ViewData["lang"] = "es";
        }

        /// <summary>
        /// This method sends the values got from the form posted to the View.
        /// </summary>
        private void SetPaymentParametersFromForm()
        {
            ViewData["merchantId"] = Request.Form["merchantId"];
            ViewData["apikey"] = Request.Form["apikey"];
            ViewData["detail"] = Request.Form["detail"];
            ViewData["type"] = Request.Form["type"];
            ViewData["currency"] = Request.Form["currency"];
            ViewData["amount"] = Request.Form["amount"];
            ViewData["country"] = Request.Form["country"];
            ViewData["lang"] = Request.Form["lang"];
        }
    }
}
