using Avalonia;
using Avalonia.Controls;
using NSModCreator.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSModCreator.Localizations
{
    public class LocalizationAttachedPropertyHolder
    {
        public static AvaloniaProperty<string> UidProperty =
            AvaloniaProperty.RegisterAttached<LocalizationAttachedPropertyHolder, AvaloniaObject, string>("Uid");

        static LocalizationAttachedPropertyHolder()
        {
            TextBlock.TextProperty.Changed.Subscribe(next =>
            {
                var uid = GetUid((AvaloniaObject)next.Sender);
                if (uid != null) 
                {
                    next.Sender.SetValue(TextBlock.TextProperty, AppResources.ResourceManager.GetString(uid.ToString()));
                }
            });

            ContentControl.ContentProperty.Changed.Subscribe(next =>
            {
                var uid = GetUid((AvaloniaObject)next.Sender);
                if (uid != null)
                {
                    next.Sender.SetValue(ContentControl.ContentProperty, AppResources.ResourceManager.GetString(uid.ToString()));
                }
            });

            TextBox.WatermarkProperty.Changed.Subscribe(next =>
            {
                var uid = GetUid((AvaloniaObject)next.Sender);
                if (uid != null)
                {
                    next.Sender.SetValue(TextBox.WatermarkProperty, AppResources.ResourceManager.GetString(uid.ToString()));
                }
            });
        }

        public static void SetUid(AvaloniaObject target, string value)
        {
            target.SetValue(UidProperty, value);
        }

        public static string GetUid(AvaloniaObject target)
        {
            return (string)target.GetValue(UidProperty);
        }
    }
}
