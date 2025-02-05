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

        public LabelsManagerVM(ILabelRepository labelRepository)
        {
            _labelRepository = (LabelRepository)labelRepository;

            LoadLabels();
        }

        public async void LoadLabels()
        {
            Labels = await _labelRepository.GetAllLabels();
        }

        public void AddOrUpdateLabel(bool isUpdate)
        {
            LabelEditVM vm = new(_labelRepository);

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

            labelEdit.Show();

            labelEdit.Closed += (sender, e) =>
            {
                LoadLabels();
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
