// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Responder.iOS.firehall.net {
    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;
    
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="WebService1Soap", Namespace="http://tempuri.org/")]
    public partial class WebService1 : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback RespondingOperationCompleted;
        
        /// CodeRemarks
        public WebService1() {
            this.Url = "http://firehall.net/api/Respond.asmx";
        }
        
        public WebService1(string url) {
            this.Url = url;
        }
        
        /// CodeRemarks
        public event RespondingCompletedEventHandler RespondingCompleted;
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Responding", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Responding(string sCompanyID, string sDeviceID, decimal dLatitude, decimal dLongitude) {
            object[] results = this.Invoke("Responding", new object[] {
                        sCompanyID,
                        sDeviceID,
                        dLatitude,
                        dLongitude});
            return ((string)(results[0]));
        }
        
        /// CodeRemarks
        public void RespondingAsync(string sCompanyID, string sDeviceID, decimal dLatitude, decimal dLongitude) {
            this.RespondingAsync(sCompanyID, sDeviceID, dLatitude, dLongitude, null);
        }
        
        /// CodeRemarks
        public void RespondingAsync(string sCompanyID, string sDeviceID, decimal dLatitude, decimal dLongitude, object userState) {
            if ((this.RespondingOperationCompleted == null)) {
                this.RespondingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRespondingOperationCompleted);
            }
            this.InvokeAsync("Responding", new object[] {
                        sCompanyID,
                        sDeviceID,
                        dLatitude,
                        dLongitude}, this.RespondingOperationCompleted, userState);
        }
        
        private void OnRespondingOperationCompleted(object arg) {
            if ((this.RespondingCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RespondingCompleted(this, new RespondingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void RespondingCompletedEventHandler(object sender, RespondingCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RespondingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RespondingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}
