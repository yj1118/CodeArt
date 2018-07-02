using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace MenuSubsystem
{
    [SafeAccess]
    public class MenuMapper : DataMapper
    {
        private IntField _left = new IntField("Left");

        private IntField _right = new IntField("Right");

        public MenuMapper()
        {

        }

        protected override IEnumerable<DbField> GetAttachFields(Type objectType, bool isSnapshot)
        {
            return new DbField[] { _left, _right };
        }

        public override void FillInsertData(DomainObject obj, DynamicData data)
        {
            var menu = (Menu)obj;

            data.Set(_left.Name, 1);
            data.Set(_right.Name, 2);
        }


        public new static readonly MenuMapper Instance = new MenuMapper();


    }
}
