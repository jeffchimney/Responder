using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Responder.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabBarCustomRenderer))]
namespace Responder.iOS
{
    public class TabBarCustomRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            foreach (UITabBarItem item in base.TabBar.Items)
            {
                item.TitlePositionAdjustment = new UIOffset(0, -15);
            }
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Set Text Font for unselected tab states
            UITextAttributes normalTextAttributes = new UITextAttributes();
            normalTextAttributes.Font = UIFont.BoldSystemFontOfSize(14.0F);//.FromName("ChalkboardSE-Light", 11.0F); // unselected
            //base.SelectedViewController.TabBarItem.TitlePositionAdjustment = new UIOffset(0, -15);

            UITabBarItem.Appearance.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
        }

        public override UIViewController SelectedViewController
        {
            get
            {
                UITextAttributes selectedTextAttributes = new UITextAttributes();
                selectedTextAttributes.Font = UIFont.BoldSystemFontOfSize(15.0F); // SELECTED
                if (base.SelectedViewController != null)
                {
                    base.SelectedViewController.TabBarItem.SetTitleTextAttributes(selectedTextAttributes, UIControlState.Normal);
                    base.SelectedViewController.TabBarItem.TitlePositionAdjustment = new UIOffset(0, -15);
                }
                return base.SelectedViewController;
            }
            set
            {
                base.SelectedViewController = value;

                foreach (UIViewController viewController in base.ViewControllers)
                {
                    UITextAttributes normalTextAttributes = new UITextAttributes();
                    normalTextAttributes.Font = UIFont.BoldSystemFontOfSize(14.0F); // unselected
                    base.SelectedViewController.TabBarItem.TitlePositionAdjustment = new UIOffset(0, -15);

                    viewController.TabBarItem.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);
                }
            }
        }
    }
}