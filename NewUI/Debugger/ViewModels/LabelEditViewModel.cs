﻿using Avalonia.Controls;
using Dock.Model.ReactiveUI.Controls;
using Mesen.Debugger.Labels;
using Mesen.Interop;
using Mesen.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Mesen.Debugger.ViewModels
{
	public class LabelEditViewModel : DisposableViewModel
	{
		[Reactive] public ReactiveCodeLabel Label { get; set; }

		[ObservableAsProperty] public bool OkEnabled { get; }
		[ObservableAsProperty] public string MaxAddress { get; } = "";
		public Enum[] AvailableMemoryTypes { get; private set; } = Array.Empty<Enum>();

		[Obsolete("For designer only")]
		public LabelEditViewModel() : this(new CodeLabel()) { }

		public LabelEditViewModel(CodeLabel label, CodeLabel? originalLabel = null)
		{
			Label = new ReactiveCodeLabel(label);

			if(Design.IsDesignMode) {
				return;
			}

			AvailableMemoryTypes = Enum.GetValues<SnesMemoryType>().Where(t => t.SupportsLabels() && DebugApi.GetMemorySize(t) > 0).Cast<Enum>().ToArray();

			AddDisposable(this.WhenAnyValue(x => x.Label.MemoryType, (memoryType) => {
				int maxAddress = DebugApi.GetMemorySize(memoryType) - 1;
				if(maxAddress <= 0) {
					return "(unavailable)";
				} else {
					return "(Max: $" + maxAddress.ToString("X4") + ")";
				}
			}).ToPropertyEx(this, x => x.MaxAddress));

			AddDisposable(this.WhenAnyValue(x => x.Label.Label, x => x.Label.Comment, x => x.Label.Length, x => x.Label.MemoryType, x => x.Label.Address, (label, comment, length, memoryType, address) => {
				CodeLabel? sameLabel = LabelManager.GetLabel(label);
				int maxAddress = DebugApi.GetMemorySize(memoryType) - 1;

				for(UInt32 i = 0; i < length; i++) {
					CodeLabel? sameAddress = LabelManager.GetLabel(address + i, memoryType);
					if(sameAddress != null) {
						if(originalLabel == null) {
							//A label already exists and we're not editing an existing label, so we can't add it
							return false;
						} else {
							if(sameAddress.Label != originalLabel.Label && !sameAddress.Label.StartsWith(originalLabel.Label + "+")) {
								//A label already exists, we're trying to edit an existing label, but the existing label
								//and the label we're editing aren't the same label.  Can't override an existing label with a different one.
								return false;
							}
						}
					}
				}

				return
					length >= 1 && length <= 65536 &&
					address + (length - 1) <= maxAddress &&
					(sameLabel == null || sameLabel == originalLabel)
					&& (label.Length > 0 || comment.Length > 0)
					&& !comment.Contains('\x1')
					&& (label.Length == 0 || LabelManager.LabelRegex.IsMatch(label));
			}).ToPropertyEx(this, x => x.OkEnabled));
		}

		public void Commit()
		{
			Label.Commit();
		}

		public class ReactiveCodeLabel : ReactiveObject
		{
			private CodeLabel _originalLabel;

			public ReactiveCodeLabel(CodeLabel label)
			{
				_originalLabel = label;

				Address = label.Address;
				Label = label.Label;
				Comment = label.Comment;
				MemoryType = label.MemoryType;
				Flags = label.Flags;
				Length = label.Length;
			}

			public void Commit()
			{
				_originalLabel.Address = Address;
				_originalLabel.Label = Label;
				_originalLabel.Comment = Comment;
				_originalLabel.MemoryType = MemoryType;
				_originalLabel.Flags = Flags;
				_originalLabel.Length = Length;
			}

			[Reactive] public UInt32 Address { get; set; }
			[Reactive] public SnesMemoryType MemoryType { get; set; }
			[Reactive] public string Label { get; set; } = "";
			[Reactive] public string Comment { get; set; } = "";
			[Reactive] public CodeLabelFlags Flags { get; set; }
			[Reactive] public UInt32 Length { get; set; } = 1;
		}
	}
}
