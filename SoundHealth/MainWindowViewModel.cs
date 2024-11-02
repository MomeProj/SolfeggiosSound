using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using SoundHealth.Commanding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.Intrinsics.Arm;

namespace SoundHealth
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		private IWavePlayer? player;
		private readonly SineWaveProvider sineProvider;

		public ICommand PlayCommand { get; private set; }
		public ICommand PauseCommand { get; private set; }
		public ICommand StopCommand { get; private set; }
		public ICommand AUMCommand { get; private set; }
		public ICommand OMCommand { get; private set; }
		public ICommand HAMCommand { get; private set; }
		public ICommand YAMCommand { get; private set; }
		public ICommand RAMCommand { get; private set; }
		public ICommand VAMCommand { get; private set; }
		public ICommand LAMCommand { get; private set; }

		public float Volume
		{
			get { return sineProvider.Volume; }
			set
			{
				if (sineProvider.Volume == value)
					return;
				sineProvider.Volume = value;
				OnPropertyChanged(nameof(Volume));
				OnPropertyChanged(nameof(VolumeLabel));
			}
		}

		public string VolumeLabel => $"{(int)(Volume * 100.0)}%";

		public string FrequencyLabel => $"{(int)Frequency}Hz";

		public double Frequency
		{
			get { return sineProvider.Frequency; }
			set
			{
				if (sineProvider.Frequency == value)
					return;
				sineProvider.Frequency = value;
				OnPropertyChanged(nameof(Frequency));
				OnPropertyChanged(nameof(FrequencyLabel));
			}
		}


		public MainWindowViewModel()
		{
			PlayCommand = new RelayCommand(Play);
			PauseCommand = new RelayCommand(Pause);
			StopCommand = new RelayCommand(Stop);
			AUMCommand = new RelayCommand(AUMSound);
			OMCommand = new RelayCommand(OMSound);
			HAMCommand = new RelayCommand(HAMSound);
			YAMCommand = new RelayCommand(YAMSound);
			RAMCommand = new RelayCommand(RAMSound);
			VAMCommand = new RelayCommand(VAMSound);
			LAMCommand = new RelayCommand(LAMSound);

			sineProvider = new SineWaveProvider();
		}


		private void Play()
		{
			if (player == null)
			{
				var waveOutEvent = new WaveOutEvent
				{
					NumberOfBuffers = 2,
					DesiredLatency = 100
				};
				player = waveOutEvent;
				player.Init(new SampleToWaveProvider(sineProvider));
			}

			player.Play();
		}

		private void Stop()
		{
			if (player == null)
				return;
			player.Dispose();
			player = null;
		}

		private void Pause()
		{
			player?.Pause();
		}

		private void LAMSound()
		{
			Frequency = 369.0;
		}
		private void VAMSound()
		{
			Frequency = 417.0;
		}
		private void RAMSound()
		{
			Frequency = 528.0;
		}
		private void YAMSound()
		{
			Frequency = 639.0;
		}
		private void HAMSound()
		{
			Frequency = 741.0;
		}
		private void OMSound()
		{
			Frequency = 852.0;
		}
		private void AUMSound()
		{
			Frequency = 932.0;
		}







		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
