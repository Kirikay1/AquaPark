using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AquaPark.Services
{
    public static class InputMaskBehavior
    {
        public static readonly DependencyProperty NumericOnlyProperty =
            DependencyProperty.RegisterAttached("NumericOnly", typeof(bool), typeof(InputMaskBehavior), new PropertyMetadata(false, OnNumericOnlyChanged));

        public static readonly DependencyProperty DecimalOnlyProperty =
            DependencyProperty.RegisterAttached("DecimalOnly", typeof(bool), typeof(InputMaskBehavior), new PropertyMetadata(false, OnDecimalOnlyChanged));

        public static readonly DependencyProperty PhoneOnlyProperty =
            DependencyProperty.RegisterAttached("PhoneOnly", typeof(bool), typeof(InputMaskBehavior), new PropertyMetadata(false, OnPhoneOnlyChanged));

        public static readonly DependencyProperty TimeOnlyProperty =
            DependencyProperty.RegisterAttached("TimeOnly", typeof(bool), typeof(InputMaskBehavior), new PropertyMetadata(false, OnTimeOnlyChanged));

        public static bool GetNumericOnly(DependencyObject obj) => (bool)obj.GetValue(NumericOnlyProperty);
        public static void SetNumericOnly(DependencyObject obj, bool value) => obj.SetValue(NumericOnlyProperty, value);

        public static bool GetDecimalOnly(DependencyObject obj) => (bool)obj.GetValue(DecimalOnlyProperty);
        public static void SetDecimalOnly(DependencyObject obj, bool value) => obj.SetValue(DecimalOnlyProperty, value);

        public static bool GetPhoneOnly(DependencyObject obj) => (bool)obj.GetValue(PhoneOnlyProperty);
        public static void SetPhoneOnly(DependencyObject obj, bool value) => obj.SetValue(PhoneOnlyProperty, value);

        public static bool GetTimeOnly(DependencyObject obj) => (bool)obj.GetValue(TimeOnlyProperty);
        public static void SetTimeOnly(DependencyObject obj, bool value) => obj.SetValue(TimeOnlyProperty, value);

        private static void OnNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleHandler(d, e, NumericPreviewTextInput, NumericPaste);
        }

        private static void OnDecimalOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleHandler(d, e, DecimalPreviewTextInput, DecimalPaste);
        }

        private static void OnPhoneOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleHandler(d, e, PhonePreviewTextInput, PhonePaste);
        }

        private static void OnTimeOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleHandler(d, e, TimePreviewTextInput, TimePaste);
        }

        private static void ToggleHandler(DependencyObject d, DependencyPropertyChangedEventArgs e, TextCompositionEventHandler inputHandler, DataObjectPastingEventHandler pasteHandler)
        {
            if (d is not TextBox textBox)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += inputHandler;
                DataObject.AddPastingHandler(textBox, pasteHandler);
            }
            else
            {
                textBox.PreviewTextInput -= inputHandler;
                DataObject.RemovePastingHandler(textBox, pasteHandler);
            }
        }

        private static void NumericPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private static void DecimalPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9,.]+$");
        }

        private static void PhonePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9+\-() ]+$");
        }

        private static void TimePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9:]+$");
        }

        private static void NumericPaste(object sender, DataObjectPastingEventArgs e)
        {
            ValidatePaste(e, value => value.All(char.IsDigit));
        }

        private static void DecimalPaste(object sender, DataObjectPastingEventArgs e)
        {
            ValidatePaste(e, value => Regex.IsMatch(value, @"^[0-9,.]+$"));
        }

        private static void PhonePaste(object sender, DataObjectPastingEventArgs e)
        {
            ValidatePaste(e, value => Regex.IsMatch(value, @"^[0-9+\-() ]+$"));
        }

        private static void TimePaste(object sender, DataObjectPastingEventArgs e)
        {
            ValidatePaste(e, value => Regex.IsMatch(value, @"^[0-9:]+$"));
        }

        private static void ValidatePaste(DataObjectPastingEventArgs e, System.Func<string, bool> validator)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string value = (string)e.DataObject.GetData(typeof(string));

            if (!validator(value))
            {
                e.CancelCommand();
            }
        }
    }
}
