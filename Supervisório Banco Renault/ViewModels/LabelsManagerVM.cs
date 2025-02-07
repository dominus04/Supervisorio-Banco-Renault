using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using Supervisório_Banco_Renault.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{


    public class LabelsManagerVM : BaseVM
    {

        #region Properties

        private readonly IServiceProvider _serviceProvider;

        private ObservableCollection<Label>? _labels;
        public ObservableCollection<Label>? Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        private Label? _selectedLabel;
        public Label? SelectedLabel
        {
            get { return _selectedLabel; }
            set
            {
                _selectedLabel = value;
                OnPropertyChanged(nameof(SelectedLabel));
            }
        }

        #endregion


        private readonly LabelRepository _labelRepository;

        public LabelsManagerVM(ILabelRepository labelRepository, IServiceProvider serviceProvider)
        {
            _labelRepository = (LabelRepository)labelRepository;
            _serviceProvider = serviceProvider;

            LoadLabels();
        }

        public async void LoadLabels()
        {
            Labels = await _labelRepository.GetAllLabels();
        }

        public void AddOrUpdateLabel(bool isUpdate)
        {
            LabelEditVM vm = new(_labelRepository);

            OP20_MainWindowVM mainVM = (OP20_MainWindowVM)_serviceProvider.GetService(typeof(OP20_MainWindowVM))!;

            if (isUpdate && SelectedLabel != null)
            {
                vm = new(_labelRepository, SelectedLabel);
            }
            else if (isUpdate && SelectedLabel == null)
            {
                return;
            }

            LabelEdit labelEdit = new()
            {
                DataContext = vm,
                Top = 140
            };

            labelEdit.Left = (1920 - labelEdit.Width) / 2;

            mainVM.ScreenControl = false;

            labelEdit.Show();

            labelEdit.Closed += (sender, e) =>
            {
                LoadLabels();
                mainVM.ScreenControl = true;
            };
        }

        public async void RemoveLabel()
        {
            if(SelectedLabel != null)
            {
                try
                {
                    await _labelRepository.RemoveLabel(SelectedLabel);
                    LoadLabels();
                }catch(Exception)
                {
                    MessageBox.Show("Erro ao remover a etiqueta", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione uma etiqueta para remover", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
