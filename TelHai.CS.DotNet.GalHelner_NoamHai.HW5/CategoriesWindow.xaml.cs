using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TelHai.CS.DotNet.GalHelner_NoamHai.HW5.Models;
using TelHai.CS.DotNet.GalHelner_NoamHai.HW5.Repositories;

namespace TelHai.CS.DotNet.GalHelner_NoamHai.HW5
{
    /// <summary>
    /// Interaction logic for CategoriesWindow.xaml
    /// </summary>
    public partial class CategoriesWindow : Window
    {
        private CategorySqlRepository _repository;
        public CategoriesWindow()
        {
            InitializeComponent();
            _repository = CategorySqlRepository.GetInstance();
            Loaded += async (s, e) => await LoadCategories();
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _repository.GetAll();
                CategoryDataGrid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string name = CategoryNameTextBox.Text;
            string parentName = ParentCategoryNameTextBox.Text;
            int parentId = -1;

            if (name == String.Empty)
            {
                MessageBox.Show("Please enter category name!");
                return;
            }

            Category? sameNameCategory = await _repository.GetByName(name);
            if (sameNameCategory != null)
            {
                MessageBox.Show("Category named: " + name + " is already exist!");
                return;
            }

            if (parentName != String.Empty)
            {
                Category? parent = await _repository.GetByName(parentName);
                if (parent != null)
                {
                    parentId = parent.Id;
                } 
                else
                {
                    MessageBox.Show("There is no category named: " + parentName);
                    return;
                }
            }

            Category category = new Category()
            {
                Name = name,
                ParentId = parentId
            };

            await _repository.Add(category);

            // re-render the data grid
            await LoadCategories();

            // clear input fields
            CategoryNameTextBox.Text = String.Empty;
            ParentCategoryNameTextBox.Text = String.Empty;
        }

        private async void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CategoryDataGrid.SelectedItem is Category categoryItem)
                {
                    BugSqlRepository bugSqlRepository = BugSqlRepository.GetInstance();
                    List<Bug> bugs = await bugSqlRepository.GetBugsByCategoryId(categoryItem.Id);
                    if (bugs.Count > 0)
                    {
                        MessageBox.Show("Category cannot be deleted, there is bugs with this category!");
                        return;
                    }

                    await _repository.Delete(categoryItem.Id);

                    // re-render the data grid
                    await LoadCategories();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void CategoryDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                int columnIndex = e.Column.DisplayIndex;
                var editedItem = e.Row.DataContext;
                var editedCellValue = (e.EditingElement as TextBox)?.Text;
                var categoryItem = editedItem as Category;

                if (categoryItem != null)
                {
                    switch (columnIndex)
                    {
                        case 0:
                            MessageBox.Show("Category ID cannot be changed!");
                            // re-render the data grid
                            await LoadCategories();
                            return;
                        case 1:
                            if (editedCellValue != null && editedCellValue != string.Empty)
                            {
                                Category? sameNameCategory = await _repository.GetByName(editedCellValue);
                                if (sameNameCategory != null)
                                {
                                    MessageBox.Show("Category named: " + editedCellValue + " is already exist!");
                                    // re-render the data grid
                                    await LoadCategories();
                                    return;
                                }
                                categoryItem.Name = editedCellValue;
                            }
                            else
                            {
                                MessageBox.Show("Please enter category name!");
                                // re-render the data grid
                                await LoadCategories();
                                return;
                            }
                            break;
                        case 2:
                            string? parentName = editedCellValue;
                            categoryItem.ParentId = -1;
                            if (parentName != null && parentName != string.Empty)
                            {
                                Category? parent = await _repository.GetByName(parentName);
                                if (parent != null)
                                {
                                    categoryItem.ParentId = parent.Id;
                                }
                                else
                                {
                                    MessageBox.Show("There is no category named: " + parentName);
                                    // re-render the data grid
                                    await LoadCategories();
                                    return;
                                }
                            }
                            break;
                    }

                    // update category in DB
                    await _repository.Update(categoryItem.Id, categoryItem);

                    // re-render the data grid
                    await LoadCategories();
                }
            }
        }
    }
}
