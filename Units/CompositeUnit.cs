using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Units
{
    public enum Unit
    {
        Unknown = -1,
        None = 0, // безразмерная величина, т.е. коэффициент
        Meters,
        Seconds,
        Kilograms,
        DegreesC
    }

    public class CompositeUnit
    {
        internal Dictionary<Unit, int> _components;

        public CompositeUnit()
        {
            _components = new Dictionary<Unit, int>();
        }

        public CompositeUnit(Unit simple)
            : this()
        {
            _components.Add(simple, 1);
        }

        public CompositeUnit(Unit simple, int power)
            : this()
        {
            _components.Add(simple, power);
        }

        public CompositeUnit(CompositeUnit u, Unit element, int power)
            : this()
        {
            foreach (Unit el in u._components.Keys)
            {
                _components.Add(el, u._components[el]);
            }
            _components.Add(element, power);
        }

        public bool IsSimple
        {
            get 
            { 
               IEnumerator<Unit> e = _components.Keys.GetEnumerator();
               e.Reset();
               e.MoveNext();
               return (_components.Count == 0) || ((_components.Count == 1) && (_components[e.Current] == 1));
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CompositeUnit)) return false;
            CompositeUnit cu = (CompositeUnit)obj;
            if (cu._components.Count != this._components.Count) return false;
            foreach (Unit el in _components.Keys)
            {
                if (!cu._components.ContainsKey(el)) return false;
                if (cu._components[el] != this._components[el]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // todo: implement ToString()

        public Unit TryReduceToSimple()
        {
            if (!IsSimple) throw new InvalidOperationException(string.Format("Cannot reduce {0} to a simple unit.",this.ToString()));
            if (_components.Count == 0) return Unit.None;
            IEnumerator<Unit> e = _components.Keys.GetEnumerator();
            e.Reset();
            e.MoveNext();
            if ((_components.Count == 1) && (_components[e.Current] == 1)) return e.Current;
            else return Unit.Unknown; //never actually returned
        }

        public CompositeUnit Clone()
        {
            CompositeUnit u = new CompositeUnit();
            foreach (Unit el in this._components.Keys)
            {
                u._components.Add(el, this._components[el]);
            }
            return u;
        }

        public CompositeUnit Inverse()
        {
            CompositeUnit u = new CompositeUnit();
            foreach (Unit el in this._components.Keys)
            {
                u._components.Add(el, -this._components[el]);
            }
            return u;
        }

        public static CompositeUnit operator *(CompositeUnit u1, CompositeUnit u2)
        {
            CompositeUnit u = u1.Clone();
            foreach (Unit el in u2._components.Keys)
            {
                if (u._components.ContainsKey(el))
                    u._components[el] += u2._components[el];
                else u._components.Add(el, u2._components[el]);
                if (u._components[el] == 0) u._components.Remove(el);
            }
            return u;
        }

        public static CompositeUnit operator /(CompositeUnit u1, CompositeUnit u2)
        {
            CompositeUnit u = u1.Clone();
            foreach (Unit el in u2._components.Keys)
            {
                if (u._components.ContainsKey(el))
                    u._components[el] -= u2._components[el];
                else u._components.Add(el, -u2._components[el]);
                if (u._components[el] == 0) u._components.Remove(el);
            }
            return u;
        }

        public static CompositeUnit operator *(CompositeUnit u1, Unit u2)
        {
            return u1 * (new CompositeUnit(u2));
        }

        public static CompositeUnit operator /(CompositeUnit u1, Unit u2)
        {
            return u1 / (new CompositeUnit(u2));
        }
    }
}
