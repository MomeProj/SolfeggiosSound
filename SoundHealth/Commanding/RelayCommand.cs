using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoundHealth.Commanding
{
	internal class RelayCommand(Action execute) : ICommand
	{
		private readonly Action execute = execute;
		public event EventHandler? CanExecuteChanged;

		public bool CanExecute(object? parameter)
		{
			return true;
		}

		public void Execute(object? parameter)
		{
			this.execute();
		}
	}
}
