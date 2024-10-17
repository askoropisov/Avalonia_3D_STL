using Avalonia.Controls;
using Avalonia_3D_STL.ViewModels;
using System;
using System.Collections.Generic;

namespace Avalonia_3D_STL.Views;

public partial class MenuView : UserControl
{
    public MenuView()
    {
        InitializeComponent();

        this.Get<Button>("OpenFile").Click += async delegate
        {
            FileDialogFilter filter = new()
            {
                Name = "STL",
                Extensions = { "STL" },
            };

            var result = await new OpenFileDialog()
            {
                Title = "Загрузить файл",
                Filters = new List<FileDialogFilter> { filter },
            }.ShowAsync(GetWindow());

            try
            {
                if (result[0] != null && DataContext is MenuViewModel vm)
                {
                    await vm.LoadSTL(result[0]);
                }

            }
            catch (Exception ex) { }
        };
    }

    private Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");
}