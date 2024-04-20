using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace QAChat {
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateControls();
        }
        public void GenerateControls()
        {
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            for (int i = 0; i < Properties.Settings.Default.Properties.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid1.RowDefinitions.Add(row);

            }
            int rowCounter = 0;
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                TextBlock block = new TextBlock();
                block.Text = currentProperty.Name;
                block.Margin = new Thickness(5);
                Grid.SetRow(block, rowCounter);
                Grid.SetColumn(block, 0);
                grid1.Children.Add(block);

                TextBox textBox = new TextBox();
                textBox.Text = Properties.Settings.Default[currentProperty.Name].ToString();
                textBox.Margin = new Thickness(5);
                textBox.Name = currentProperty.Name;
                Grid.SetRow(textBox, rowCounter);
                Grid.SetColumn(textBox, 1);
                
                grid1.RegisterName(textBox.Name, textBox);
                grid1.Children.Add(textBox);
                rowCounter++;

            }
        }
        public void btnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                // currentPropertyの型がbool型の場合
                if (currentProperty.PropertyType == typeof(bool))
                {
                    Properties.Settings.Default[currentProperty.Name] = bool.Parse(((TextBox)grid1.FindName(currentProperty.Name)).Text);
                }
                // currentPropertyの型がint型の場合
                else if (currentProperty.PropertyType == typeof(int))
                {
                    Properties.Settings.Default[currentProperty.Name] = int.Parse(((TextBox)grid1.FindName(currentProperty.Name)).Text);
                }
                else
                {
                    Properties.Settings.Default[currentProperty.Name] = ((TextBox)grid1.FindName(currentProperty.Name)).Text;
                }
            }
            Properties.Settings.Default.Save();
            this.Close();
        }
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
