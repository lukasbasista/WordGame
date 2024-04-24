using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WordGame.Helpers
{
    public static class ButtonSoundEffect
    {
        public static readonly DependencyProperty IsSoundEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsSoundEnabled",
                typeof(bool),
                typeof(ButtonSoundEffect),
                new PropertyMetadata(false, OnIsSoundEnabledChanged));

        public static void SetIsSoundEnabled(DependencyObject obj, bool value) => obj.SetValue(IsSoundEnabledProperty, value);
        public static bool GetIsSoundEnabled(DependencyObject obj) => (bool)obj.GetValue(IsSoundEnabledProperty);

        private static void OnIsSoundEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if ((bool)e.NewValue)
                {
                    button.Click += PlaySoundOnClick;
                }
                else
                {
                    button.Click -= PlaySoundOnClick;
                }
            }
        }

        private static void PlaySoundOnClick(object sender, RoutedEventArgs e)
        {
            SoundManager.PlayClickSound();
        }
    }
}
