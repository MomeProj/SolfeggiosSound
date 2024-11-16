using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoundHealth
{
	public class StereoSineWaveProvider : WaveProvider32
	{
		private readonly float[] waveTable;
		private float leftChannelFrequency;    // 左聲道頻率
		private float rightChannelFrequency;   // 右聲道頻率
		private double leftChannelPhase;
		private double leftChannelCurrentPhaseStep;
		private double leftChannelTargetPhaseStep;
		private double leftChannelPhaseStepDelta;
		private bool leftChannelSeekFreq;
		private double rightChannelPhase;
		private double rightChannelCurrentPhaseStep;
		private double rightChannelTargetPhaseStep;
		private double rightChannelPhaseStepDelta;
		private bool rightChannelSeekFreq;

		public double PortamentoTime { get; set; }
		public float LeftChannelFrequency
		{
			get
			{
				return leftChannelFrequency;
			}
			set
			{
				leftChannelFrequency = value;
				leftChannelSeekFreq = true;
			}
		}

		public float RightChannelFrequency
		{
			get
			{
				return rightChannelFrequency;
			}
			set
			{
				rightChannelFrequency = value;
				rightChannelSeekFreq = true;
			}
		}

		public float Volume { get; set; }


		public StereoSineWaveProvider(int sampleRate = 44100)
			: base(sampleRate, 2)
		{
			
			waveTable = new float[sampleRate];

			for (int index = 0; index < sampleRate; ++index)
			{
				waveTable[index] = (float)Math.Sin(2 * Math.PI * (double)index / sampleRate);
			}
			// For sawtooth instead of sine: waveTable[index] = (float)index / sampleRate;
			PortamentoTime = 0.5; // thought this was in seconds, but glide seems to take a bit longer

			LeftChannelFrequency = 306f;
			RightChannelFrequency = 300f;
			Volume = 0.25f;
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			if (leftChannelSeekFreq) // process frequency change only once per call to Read
			{
				leftChannelTargetPhaseStep = waveTable.Length * (leftChannelFrequency / WaveFormat.SampleRate);

				leftChannelPhaseStepDelta = (leftChannelTargetPhaseStep - leftChannelCurrentPhaseStep) / (WaveFormat.SampleRate * PortamentoTime);
				leftChannelSeekFreq = false;
			}

			if (rightChannelSeekFreq) // process frequency change only once per call to Read
			{
				rightChannelTargetPhaseStep = waveTable.Length * (rightChannelFrequency / WaveFormat.SampleRate);

				rightChannelPhaseStepDelta = (rightChannelTargetPhaseStep - rightChannelCurrentPhaseStep) / (WaveFormat.SampleRate * PortamentoTime);
				rightChannelSeekFreq = false;
			}

			var vol = Volume; // process volume change only once per call to Read

			for (int n = 0; n < count; n += 2)
			{
				int leftChannelWaveTableIndex = (int)leftChannelPhase % waveTable.Length;

				buffer[n + offset] = waveTable[leftChannelWaveTableIndex] * vol;

				leftChannelPhase += leftChannelCurrentPhaseStep;

				if (leftChannelPhase > waveTable.Length)
				{
					leftChannelPhase -= waveTable.Length;
				}

				if (leftChannelCurrentPhaseStep != leftChannelTargetPhaseStep)
				{
					leftChannelCurrentPhaseStep += leftChannelPhaseStepDelta;

					if (leftChannelPhaseStepDelta > 0.0 && leftChannelCurrentPhaseStep > leftChannelTargetPhaseStep)
					{
						leftChannelCurrentPhaseStep = leftChannelTargetPhaseStep;
					}
					else if (leftChannelPhaseStepDelta < 0.0 && leftChannelCurrentPhaseStep < leftChannelTargetPhaseStep)
					{
						leftChannelCurrentPhaseStep = leftChannelTargetPhaseStep;
					}
				}

				int rightChannelWaveTableIndex = (int)rightChannelPhase % waveTable.Length;

				buffer[n + offset + 1] = waveTable[rightChannelWaveTableIndex] * vol;

				rightChannelPhase += rightChannelCurrentPhaseStep;

				if (rightChannelPhase > waveTable.Length)
				{
					rightChannelPhase -= waveTable.Length;
				}

				if (rightChannelCurrentPhaseStep != rightChannelTargetPhaseStep)
				{
					rightChannelCurrentPhaseStep += rightChannelPhaseStepDelta;

					if (rightChannelPhaseStepDelta > 0.0 && rightChannelCurrentPhaseStep > rightChannelTargetPhaseStep)
					{
						rightChannelCurrentPhaseStep = rightChannelTargetPhaseStep;
					}
					else if (rightChannelPhaseStepDelta < 0.0 && rightChannelCurrentPhaseStep < rightChannelTargetPhaseStep)
					{
						rightChannelCurrentPhaseStep = rightChannelTargetPhaseStep;
					}
				}
			}

			return count;
		}
	}

}
