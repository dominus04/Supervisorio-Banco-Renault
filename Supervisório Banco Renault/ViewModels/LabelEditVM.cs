using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class LabelEditVM : BaseVM
    {

        #region Properties

        private readonly LabelRepository _labelRepository;

        private readonly bool isUpdate = false;

        private Label? _label;
        public Label? Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged(nameof(Label));
            }
        }

        private ObservableCollection<string> _julianDateFormats =
        [
            "YYDDD",
            "DDDYY"
        ];

        public ObservableCollection<string> JulianDateFormats
        {
            get => _julianDateFormats;
            set
            {
                _julianDateFormats = value;
                OnPropertyChanged(nameof(JulianDateFormats));
            }
        }
        
        private ObservableCollection<string> _dateFormats =
        [
            "dd/MM/yyyy",
            "MM/dd/yyyy"
        ];
        public ObservableCollection<string> DateFormats
        {
            get => _dateFormats;
            set
            {
                _dateFormats = value;
                OnPropertyChanged(nameof(DateFormats));
            }
        }

        private ObservableCollection<string> _timeFormats =
        [
            "HH:mm:ss",
            "hh:mm"
        ];

        public ObservableCollection<string> TimeFormats
        {
            get => _timeFormats;
            set
            {
                _timeFormats = value;
                OnPropertyChanged(nameof(TimeFormats));
            }
        }

        private ObservableCollection<int> _sequentialFormats =
        [
            1,
            2,
            3,
            4,
            5
        ];
        public ObservableCollection<int> SequentialFormats
        {
            get => _sequentialFormats;
            set
            {
                _sequentialFormats = value;
                OnPropertyChanged(nameof(SequentialFormats));
            }
        }

        #endregion

        public LabelEditVM(ILabelRepository labelRepository, Label label) 
        {
            _labelRepository = (LabelRepository)labelRepository;
            Label = label;
            isUpdate = true;
        }

        public LabelEditVM(ILabelRepository labelRepository)
        {
            _labelRepository = (LabelRepository)labelRepository;
            Label = new();
            Label.SequentialFormat = 3;
        }

        public async Task<bool> AddOrUpdateLabel()
        {
            if (Label == null)
                return false;

            try
            {
                if (isUpdate)
                {
                    await _labelRepository.UpdateLabel(Label);
                }
                else
                {
                    await _labelRepository.AddLabel(Label);
                }
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao adicionar ou atualizar a etiqueta", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

    }
}
