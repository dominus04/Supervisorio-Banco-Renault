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
    public class OP20_TraceabilityVM : BaseVM
    {

        private readonly IOP20_TraceabilityRepository _oP20_TraceabilityRepository;

        private ObservableCollection<OP20_TraceabilityModel> _traceabilities { get; set; } = new();

        public ObservableCollection<OP20_TraceabilityModel> Traceabilities 
        { 
            get { return _traceabilities; } 
            set
            {
                _traceabilities = value;
                OnPropertyChanged(nameof(Traceabilities));
            }
        }  

        
        private string? _traceabilityCodeFilter;
        public string? TraceabilityCodeFilter
        {
            get => _traceabilityCodeFilter;
            set
            {
                _traceabilityCodeFilter = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(TraceabilityCodeFilter));
            }
        }

        private string? _radiatorCodeFilter;
        public string? RadiatorCodeFilter
        {
            get { return _radiatorCodeFilter; }
            set
            {
                _radiatorCodeFilter = value;
                _ = ApplyFilter();
                OnPropertyChanged(nameof(RadiatorCodeFilter));
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

        public OP20_TraceabilityVM(IOP20_TraceabilityRepository oP20_TraceabilityRepository)
        {
            this._oP20_TraceabilityRepository = oP20_TraceabilityRepository;
            _ = ApplyFilter();
        }

        private async Task ApplyFilter()
        {
            Traceabilities.Clear();
            Traceabilities = await _oP20_TraceabilityRepository.GetTraceabilityFiltered(TraceabilityCodeFilter, RadiatorCodeFilter, InitialDate, FinalDate);
        }


    }
}
