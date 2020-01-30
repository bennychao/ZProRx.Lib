using System;
using System.Collections;
using UniRx;

namespace ZP.Lib
{

    public class ZListCursor<T> : ZLinkProperty<int>
    {
        public ZListCursor()
        {
            this.Value = 0;
        }

        public ZListCursor(int starter)
        {
            this.Value = starter;
        }


        override protected void SetValue(int value)
        {
            int count = 1;
            var list = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;
            if (list != null)
            {
                 count= list.Count;
            }

            else if (nlist != null)
            {
                 count = nlist.Count;
            }

            if (count > 0)
            {
                base.SetValue(value % count);
            }
        }

        protected override void SetLink(IZProperty value)
        {
            base.SetLink(value);

            if (!ZPropertyMesh.IsPropertyListLike(value)) {
                throw new Exception("Not Link a PropertyList " + this.PropertyID);
            }

            var linkList = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;
            if (linkList != null)
            {
                (linkList as IZPropertyRefList).AddItemAsObservable().Subscribe(  (IZProperty prop) =>
                {
                    int index = linkList.FindIndex(p => (object)p == (object)prop.Value);
                    if (index <= this.Value)
                        this.Value++;
                }).AddTo(disposables);

                disposables.Add(new OnDeleteRefItemDisposable(linkList, (IZProperty prop, int index) =>
                {
                    //int index = linkList.FindIndex(p => (object)p == (object)prop.Value);
                    if (index < this.Value)
                        this.Value--;

                }));
            }


            else if (nlist != null)
            {
                (nlist as IZPropertyList).AddItemAsObservable().Subscribe( (IZProperty prop) =>
                {
                    int index = nlist.FindIndex(p => (object)p == (object)prop.Value);
                    if (index <= this.Value)
                        this.Value++;
                }).AddTo(disposables);

                disposables.Add(new OnDeleteItemDisposable( nlist, (IZProperty prop, int index) =>
                {
                    //int index = linkList.FindIndex(p => (object)p == (object)prop.Value);
                    if (index < this.Value)
                        this.Value--;

                }));
            }
        }

        public IZProperty CurProp()
        {
            var list = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;
            if (list != null)
            {
                int count = list.Count;

                //this.Value = (this.Value + 1) % count;
                return list[this.Value] as IZProperty;
            }


            else if (nlist != null)
            {
                int count = nlist.Count;

                //this.Value = (this.Value + 1) % count;
                return nlist[this.Value] as IZProperty;
            }

            return null;

        }

        public IZProperty NextProp() {

            var list = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;
            if (list != null)
            {
                int count = list.Count;

                this.Value = (this.Value + 1) % count;
                return list[this.Value] as IZProperty;
            }
           
            else if (nlist != null)
            {
                int count = nlist.Count;

                this.Value = (this.Value + 1) % count;
                return nlist[this.Value] as IZProperty;
            }

            return null;
        }

        public IZProperty PrevProp()
        {
            var list = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;

            if (list != null)
            {
                int count = list.Count;

                this.Value = (this.Value - 1) % count;
                return list[this.Value] as IZProperty;
            }

            else if (nlist != null)
            {
                int count = nlist.Count;

                this.Value = (this.Value - 1) % count;
                return nlist[this.Value] as IZProperty;
            }

            return null;
        }

        public int PropCount()
        {
            var list = LinkProperty as ZPropertyRefList<T>;
            var nlist = LinkProperty as ZPropertyList<T>;

            if (list != null)
            {
                int count = list.Count;
                return count;
            }

            else if (nlist != null)
            {
                int count = nlist.Count;                
                return count;
            }

            return -1;
        }

        //public static ZListCursor<T> operator ++ (ZListCursor<T> cur)
        //{
        //    return 10;
        //}
    }
}
