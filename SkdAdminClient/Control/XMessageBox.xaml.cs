using System;
using System.Linq;
using System.Windows;

namespace SkdAdminClient
{
    public class XMessageBox
    {
        /// <summary>
        /// 在当前UI线程，显示模态窗口
        /// </summary>
        public static bool Show(string message)
        {
            return ShowDialog(message, null);
        }
        /// <summary>
        /// 在当前UI线程，显示模态窗口
        /// </summary>
        public static bool Show(string message, string caption)
        {
            return ShowDialog(message, caption);
        }

        /// <summary>
        /// 在当前UI线程，显示模态窗口
        /// </summary>
        /// <param name="dialogText">提示内容</param>
        /// <param name="dialogCaption">窗口标题</param>
        /// <param name="dialogConfirmCancleButton">true：显示确认和取消两个按钮。false：显示确认按钮</param>
        /// <returns></returns>
        public static bool ShowDialog(string dialogText, string dialogCaption, bool dialogConfirmCancleButton = false)
        {

            var result = Application.Current.Dispatcher.Invoke(new Func<bool?>(() =>
            {
                var winBox = new Control.XMessageBox();
                winBox.Owner = Application.Current.Windows.Cast<Window>().FirstOrDefault(ww => ww.IsActive);
                return winBox.ShowDialog(dialogText, dialogCaption, dialogConfirmCancleButton);
            })) as bool?;

            return result ?? false;
        }
    }
}

namespace SkdAdminClient.Control
{
    /// <summary>
    /// 自定义模态窗口
    /// </summary>
    partial class XMessageBox : Window
    {
        public XMessageBox()
        {
            InitializeComponent();
        }

        #region 内部方法

        private void _CloseAndReturn(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }

        #endregion 内部方法

        /// <summary>
        /// 显示模态窗口
        /// </summary>
        /// <param name="dialogText">提示内容</param>
        /// <param name="dialogCaption">窗口标题</param>
        /// <param name="dialogConfirmCancleButton">true：显示确认和取消两个按钮。false：显示确认按钮</param>
        /// <returns></returns>
        internal bool? ShowDialog(string dialogText, string dialogCaption, bool dialogConfirmCancleButton = false)
        {
            if (!String.IsNullOrWhiteSpace(dialogText))
            {
                this.TxtText.Text = dialogText;
            }
            if (!String.IsNullOrWhiteSpace(dialogCaption))
            {
                this.TxtCaption.Text = dialogCaption;
            }
            if (!dialogConfirmCancleButton)
            {
                this.BtnCancel.Visibility = Visibility.Collapsed;
            }
            this.BtnConfirm.Focus();

            return this.ShowDialog();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            _CloseAndReturn(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _CloseAndReturn(false);
        }

        private void borderCaption_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// 按下Enter等同于确认。
        /// 按下ESC等同于取消。
        /// </summary>
        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                _CloseAndReturn(true);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                _CloseAndReturn(false);
            }
            e.Handled = true;
        }
    }
}