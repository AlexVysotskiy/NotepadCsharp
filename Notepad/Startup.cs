using Microsoft.Owin;
using Owin;
using Notepad.Models.Repository;
using Notepad.Models;
using System.Web.Services.Description;
using System;

[assembly: OwinStartupAttribute(typeof(Notepad.Startup))]
namespace Notepad
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }

}
