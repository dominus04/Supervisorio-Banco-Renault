using Supervisório_Banco_Renault.Data.Repositories;
using Supervisório_Banco_Renault.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervisório_Banco_Renault.ViewModels
{
    public class OP10_TraceabilityVM : BaseVM
    {
        private readonly IOP10_TraceabilityRepository _oP10_TraceabilityRepository;

        private ObservableCollection<OP10_TraceabilityModel> _traceabilities { get; set; } = new();

        public ObservableCollection<OP10_TraceabilityModel> Traceabilities
        {
            get { return _traceabilities; }
            set
            {
                _traceabilities = value;
                OnPropertyChanged(nameof(Traceabilities));
            }
        }


        private string? _radiatorCode;
        public string? RadiatorCode
        {
            get => _radiatorCode;
            set
            {
                _radiatorCode = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(RadiatorCode));
            }
        }


        private DateTime? _initialDate;
        public DateTime? InitialDate
        {
            get => _initialDate;
            set
            {
                _initialDate = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(InitialDate));
            }
        }


        private DateTime? _finalDate;
        public DateTime? FinalDate
        {
            get => _finalDate;
            set
            {
                if (value.HasValue)
                {
                    value = value.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                _finalDate = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(FinalDate));
            }
        }

        private int _op20Executed = 0; 

        public int OP20Executed
        {
            get => _op20Executed;
            set
            {
                _op20Executed = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(OP20Executed));
            }
        }

        public OP10_TraceabilityVM(IOP10_TraceabilityRepository oP10_TraceabilityRepository)
        {
            this._oP10_TraceabilityRepository = oP10_TraceabilityRepository;
            _ = ApplyFilter();
        }

        private async Task ApplyFilter()
        {
            Traceabilities.Clear();
            Traceabilities = await _oP10_TraceabilityRepository.GetTraceabilityFiltered(RadiatorCode!, InitialDate, FinalDate, OP20Executed);
        }

    }
}
