using System;

namespace ZP.Lib
{
    /// <summary>
    /// Z exp.
    /// </summary>
    [PropertyValueChangeAnchorClass(".CurExp")]
    public class ZExp
	{
        
		public ZPropertyList<float> Ranks = new ZPropertyList<float>();

        public ZProperty<int> CurRank = new ZProperty<int>();   //form 0 ~ Ranks.Count - 1

        public ZProperty<float> CurExp = new ZProperty<float>();

        //view
        public ZRuntimableProperty<float> CurBar = new ZRuntimableProperty<float>();

        

        /// <summary>
        /// Raises the create event.
        /// </summary>
        public void OnCreate()
        {
            CurBar.Select<ZProperty<float>>(CurExp, (CurExp) => {

                if (CurRank.Value < 0 || CurRank.Value >= Ranks.Count)
                    return 0;
                float fRet = CurExp.Value / Ranks[CurRank];
                return fRet;
            });
        }

        /// <summary>
        /// Gets or sets the exp.
        /// </summary>
        /// <value>The exp.</value>
        public float Exp{
			get{
				return CurExp.Value;
			}
			set {
				
				CurExp.Value = value;

				do {
					if (CurExp.Value > Ranks [CurRank.Value]) {
						CurExp.Value -= Ranks [CurRank.Value];

						if (CurRank.Value < Ranks.Count)
						{
							CurRank.Value++;
							break;
						}
					}else{
						break;
					}

				} while(true);



			}
		}

		/// <summary>
		/// Gets the rank.
		/// </summary>
		/// <value>The rank.</value>
		public float Rank{
			get{
				return CurRank.Value;
			}
		}

		/// <param name="d">D.</param>
		public static implicit operator float(ZExp d)  // implicit digit to byte conversion operator
		{
			return d.CurExp.Value;  // implicit conversion
		}

        /// <summary>
        /// Gets the max.
        /// </summary>
        /// <value>The max.</value>
        public float Max
        {
            get
            {
                if (CurRank.Value < 0 || CurRank.Value >= Ranks.Count)
                    return 0;
                 return Ranks[CurRank];
            }
        }

        public int MaxRank => Ranks.Count;
    }

    public class ZExpProperty : ZProperty<ZExp> { }
}

