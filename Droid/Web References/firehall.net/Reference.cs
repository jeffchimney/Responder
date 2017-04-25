// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Responder.Droid.firehall.net {
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
        
        private System.Threading.SendOrPostCallback TestOperationCompleted;
        
        private System.Threading.SendOrPostCallback RegisterOperationCompleted;
        
        private System.Threading.SendOrPostCallback LoginOperationCompleted;
        
        private System.Threading.SendOrPostCallback RespondingOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetResponsesOperationCompleted;
        
        private System.Threading.SendOrPostCallback StopRespondingOperationCompleted;
        
        /// CodeRemarks
        public WebService1() {
            this.Url = "http://firehall.net/api/Respond.asmx";
        }
        
        public WebService1(string url) {
            this.Url = url;
        }
        
        /// CodeRemarks
        public event TestCompletedEventHandler TestCompleted;
        
        /// CodeRemarks
        public event RegisterCompletedEventHandler RegisterCompleted;
        
        /// CodeRemarks
        public event LoginCompletedEventHandler LoginCompleted;
        
        /// CodeRemarks
        public event RespondingCompletedEventHandler RespondingCompleted;
        
        /// CodeRemarks
        public event GetResponsesCompletedEventHandler GetResponsesCompleted;
        
        /// CodeRemarks
        public event StopRespondingCompletedEventHandler StopRespondingCompleted;
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Test", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output Test(string sInput, int iVersion, int iSubVersion) {
            object[] results = this.Invoke("Test", new object[] {
                        sInput,
                        iVersion,
                        iSubVersion});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void TestAsync(string sInput, int iVersion, int iSubVersion) {
            this.TestAsync(sInput, iVersion, iSubVersion, null);
        }
        
        /// CodeRemarks
        public void TestAsync(string sInput, int iVersion, int iSubVersion, object userState) {
            if ((this.TestOperationCompleted == null)) {
                this.TestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestOperationCompleted);
            }
            this.InvokeAsync("Test", new object[] {
                        sInput,
                        iVersion,
                        iSubVersion}, this.TestOperationCompleted, userState);
        }
        
        private void OnTestOperationCompleted(object arg) {
            if ((this.TestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestCompleted(this, new TestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Register", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output Register(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier) {
            object[] results = this.Invoke("Register", new object[] {
                        iVersion,
                        iSubVersion,
                        sFireHallRespondID,
                        sStaffRespondID,
                        sDeviceIdentifier});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void RegisterAsync(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier) {
            this.RegisterAsync(iVersion, iSubVersion, sFireHallRespondID, sStaffRespondID, sDeviceIdentifier, null);
        }
        
        /// CodeRemarks
        public void RegisterAsync(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier, object userState) {
            if ((this.RegisterOperationCompleted == null)) {
                this.RegisterOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRegisterOperationCompleted);
            }
            this.InvokeAsync("Register", new object[] {
                        iVersion,
                        iSubVersion,
                        sFireHallRespondID,
                        sStaffRespondID,
                        sDeviceIdentifier}, this.RegisterOperationCompleted, userState);
        }
        
        private void OnRegisterOperationCompleted(object arg) {
            if ((this.RegisterCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RegisterCompleted(this, new RegisterCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Login", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output Login(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier) {
            object[] results = this.Invoke("Login", new object[] {
                        iVersion,
                        iSubVersion,
                        sFireHallRespondID,
                        sStaffRespondID,
                        sDeviceIdentifier});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void LoginAsync(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier) {
            this.LoginAsync(iVersion, iSubVersion, sFireHallRespondID, sStaffRespondID, sDeviceIdentifier, null);
        }
        
        /// CodeRemarks
        public void LoginAsync(int iVersion, int iSubVersion, string sFireHallRespondID, string sStaffRespondID, string sDeviceIdentifier, object userState) {
            if ((this.LoginOperationCompleted == null)) {
                this.LoginOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLoginOperationCompleted);
            }
            this.InvokeAsync("Login", new object[] {
                        iVersion,
                        iSubVersion,
                        sFireHallRespondID,
                        sStaffRespondID,
                        sDeviceIdentifier}, this.LoginOperationCompleted, userState);
        }
        
        private void OnLoginOperationCompleted(object arg) {
            if ((this.LoginCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LoginCompleted(this, new LoginCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Responding", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output Responding(int iVersion, int iSubVersion, string sDeviceIdentifier, decimal dLatitude, decimal dLongitude, int iTimeToHall) {
            object[] results = this.Invoke("Responding", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier,
                        dLatitude,
                        dLongitude,
                        iTimeToHall});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void RespondingAsync(int iVersion, int iSubVersion, string sDeviceIdentifier, decimal dLatitude, decimal dLongitude, int iTimeToHall) {
            this.RespondingAsync(iVersion, iSubVersion, sDeviceIdentifier, dLatitude, dLongitude, iTimeToHall, null);
        }
        
        /// CodeRemarks
        public void RespondingAsync(int iVersion, int iSubVersion, string sDeviceIdentifier, decimal dLatitude, decimal dLongitude, int iTimeToHall, object userState) {
            if ((this.RespondingOperationCompleted == null)) {
                this.RespondingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRespondingOperationCompleted);
            }
            this.InvokeAsync("Responding", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier,
                        dLatitude,
                        dLongitude,
                        iTimeToHall}, this.RespondingOperationCompleted, userState);
        }
        
        private void OnRespondingOperationCompleted(object arg) {
            if ((this.RespondingCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RespondingCompleted(this, new RespondingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetResponses", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output GetResponses(int iVersion, int iSubVersion, string sDeviceIdentifier) {
            object[] results = this.Invoke("GetResponses", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void GetResponsesAsync(int iVersion, int iSubVersion, string sDeviceIdentifier) {
            this.GetResponsesAsync(iVersion, iSubVersion, sDeviceIdentifier, null);
        }
        
        /// CodeRemarks
        public void GetResponsesAsync(int iVersion, int iSubVersion, string sDeviceIdentifier, object userState) {
            if ((this.GetResponsesOperationCompleted == null)) {
                this.GetResponsesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetResponsesOperationCompleted);
            }
            this.InvokeAsync("GetResponses", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier}, this.GetResponsesOperationCompleted, userState);
        }
        
        private void OnGetResponsesOperationCompleted(object arg) {
            if ((this.GetResponsesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetResponsesCompleted(this, new GetResponsesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/StopResponding", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WS_Output StopResponding(int iVersion, int iSubVersion, string sDeviceIdentifier) {
            object[] results = this.Invoke("StopResponding", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier});
            return ((WS_Output)(results[0]));
        }
        
        /// CodeRemarks
        public void StopRespondingAsync(int iVersion, int iSubVersion, string sDeviceIdentifier) {
            this.StopRespondingAsync(iVersion, iSubVersion, sDeviceIdentifier, null);
        }
        
        /// CodeRemarks
        public void StopRespondingAsync(int iVersion, int iSubVersion, string sDeviceIdentifier, object userState) {
            if ((this.StopRespondingOperationCompleted == null)) {
                this.StopRespondingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStopRespondingOperationCompleted);
            }
            this.InvokeAsync("StopResponding", new object[] {
                        iVersion,
                        iSubVersion,
                        sDeviceIdentifier}, this.StopRespondingOperationCompleted, userState);
        }
        
        private void OnStopRespondingOperationCompleted(object arg) {
            if ((this.StopRespondingCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StopRespondingCompleted(this, new StopRespondingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// CodeRemarks
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class WS_Output {
        
        /// <remarks/>
        public WS_Result Result;
        
        /// <remarks/>
        public string ErrorMessage;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<decimal> HallLatitude;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<decimal> HallLongitude;
        
        /// <remarks/>
        public WS_Response MyResponse;
        
        /// <remarks/>
        public WS_Response[] Responses;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum WS_Result {
        
        /// <remarks/>
        NA,
        
        /// <remarks/>
        OK,
        
        /// <remarks/>
        Error,
        
        /// <remarks/>
        Upgrade,
        
        /// <remarks/>
        AtHall,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class WS_Response {
        
        /// <remarks/>
        public string FullName;
        
        /// <remarks/>
        public string TimeToHall;
        
        /// <remarks/>
        public string DistanceToHall;
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void TestCompletedEventHandler(object sender, TestCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
        }
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void RegisterCompletedEventHandler(object sender, RegisterCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RegisterCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RegisterCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
        }
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void LoginCompletedEventHandler(object sender, LoginCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal LoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
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
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
        }
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void GetResponsesCompletedEventHandler(object sender, GetResponsesCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetResponsesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetResponsesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
        }
    }
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    public delegate void StopRespondingCompletedEventHandler(object sender, StopRespondingCompletedEventArgs e);
    
    /// CodeRemarks
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class StopRespondingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal StopRespondingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// CodeRemarks
        public WS_Output Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((WS_Output)(this.results[0]));
            }
        }
    }
}
