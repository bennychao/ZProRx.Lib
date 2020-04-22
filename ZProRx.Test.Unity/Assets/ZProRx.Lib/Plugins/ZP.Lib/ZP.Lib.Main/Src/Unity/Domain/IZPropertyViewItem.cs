using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{
    public interface IZRadioViewItem
    {
        void OnSelected();
        void OnUnselected();
    }

    public interface IZPropertyViewItem {

        bool IsBind { get; }

		bool Bind (IZProperty property);
        void Unbind();
		void UpdateValue(object data);
	}

	public interface IZPropertyViewRoot {

		void Bind (object root);
        void Unbind();

        bool IsBinded(object obj);

    }
}

