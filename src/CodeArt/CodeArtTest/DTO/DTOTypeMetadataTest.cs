using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DTO;

namespace CodeArtTest.DTO
{
    /// <summary>
    /// DTOTest 的摘要说明
    /// </summary>
    [TestClass]
    public class DTOTypeMetadataTest
    {
        public DTOTypeMetadataTest()
        {

        }

        [TestMethod]
        public void ParseType1()
        {
            //const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'},first:'person',menu:{person:{id,nam}},name:'string,10',parent:'menu',childs:['menu']},others:[{id:'int',name:'string',person:'menu.person'}]}";
            const string code = "{id:'int',name:'ascii,10'}";
            var metadata = DTObject.GetMetadata(code);

            var es = metadata.Entries.ToArray();

            AssertValue(es[0], "id", "int");
            AssertValue(es[1], "name", "ascii", "10");
        }

        [TestMethod]
        public void ParseType2()
        {
            const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'}}";
            var metadata = DTObject.GetMetadata(code);

            var es = metadata.Entries.ToArray();

            AssertValue(es[0], "id", "int");
            AssertValue(es[1], "name", "ascii", "10");
            AssertObject(es[2], "person", "person");
        }

        [TestMethod]
        public void ParseType3()
        {
            const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'},first:'person',menu:{person:{id:'long',name:'string,5'},name:'string,10',parent:'menu',childs:['menu']},others:[{id:'int',name:'string',person:'menu.person'}],values:['string,10'],others2:'others'}";
            var metadata = DTObject.GetMetadata(code);

            var es = metadata.Entries.ToArray();

            AssertValue(es[0], "id", "int");
            AssertValue(es[1], "name", "ascii", "10");

            AssertObject(es[2], "person", "person");
            var person = es[2] as ObjectEntry;
            var personChilds = person.Childs.ToArray();
            AssertValue(personChilds[0], "name", "string","10");
            AssertValue(personChilds[1], "sex", "byte");

            AssertObject(es[3], "first", "person");
            var first = es[3] as ObjectEntry;
            var firstChilds = first.Childs.ToArray();
            AssertValue(firstChilds[0], "name", "string", "10");
            AssertValue(firstChilds[1], "sex", "byte");

            AssertMenu(es[4]);

            AssertOthers(es[5], "others", "others");

            AssertList(es[6], "values", "values");
            var valuesItem = (es[6] as ListEntry).ItemEntry;
            AssertValue(valuesItem, "item", "string", "10");

            AssertOthers(es[7], "others2", "others");

        }

        [TestMethod]
        public void ParseType4()
        {
            //const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'},first:'person',menu:{person:{id,nam}},name:'string,10',parent:'menu',childs:['menu']},others:[{id:'int',name:'string',person:'menu.person'}]}";
            const string code = "user:{id:'int',name:'ascii,10',son:'user'}";
            var metadata = DTObject.GetMetadata(code);

            var es = metadata.Entries.ToArray();

            AssertValue(es[0], "id", "int");
            AssertValue(es[1], "name", "ascii", "10");


            AssertObject(es[2], "son", "user");
            var sonES = (es[2] as ObjectEntry).Childs.ToArray();

            AssertValue(sonES[0], "id", "int");
            AssertValue(sonES[1], "name", "ascii", "10");
            AssertObject(sonES[2], "son", "user");
        }

        private void AssertOthers(TypeEntry entry, string name, string typeName)
        {
            AssertList(entry, name, typeName);
            var othersItemEntry = (entry as ListEntry).ItemEntry;
            AssertObject(othersItemEntry, "item", "item");
            var obj = othersItemEntry as ObjectEntry;
            var childs = obj.Childs.ToArray();

            AssertValue(childs[0], "id", "int");
            AssertValue(childs[1], "name", "string");
            AssertMenuPerson(childs[2]);
        }

        private void AssertMenu(TypeEntry entry)
        {
            AssertObject(entry, "menu", "menu");
            AssertMenuBase(entry);

            var menu = entry as ObjectEntry;
            var menuChilds = menu.Childs.ToArray();

            //验证数组
            AssertList(menuChilds[3], "childs", "menu.childs");
            var menuChildsEntry = menuChilds[3] as ListEntry;
            AssertObject(menuChildsEntry.ItemEntry, "item", "menu");
            AssertMenuBase(menuChildsEntry.ItemEntry);
        }

        private void AssertMenuBase(TypeEntry entry)
        {
            var menu = entry as ObjectEntry;
            var menuChilds = menu.Childs.ToArray();


            AssertMenuPerson(menuChilds[0]);
            AssertValue(menuChilds[1], "name", "string", "10");


            AssertObject(menuChilds[2], "parent", "menu");
            var menuParent = menuChilds[2] as ObjectEntry;
            var menuParentChilds = menuParent.Childs.ToArray();
            AssertMenuPerson(menuParentChilds[0]);
            AssertValue(menuParentChilds[1], "name", "string", "10");
        }


        private void AssertMenuPerson(TypeEntry entry)
        {
            AssertObject(entry, "person", "menu.person");
            var menuPerson = entry as ObjectEntry;
            var menuPersonChilds = menuPerson.Childs.ToArray();
            AssertValue(menuPersonChilds[0], "id", "long");
            AssertValue(menuPersonChilds[1], "name", "string", "5");
        }


        private void AssertValue(TypeEntry entry, string name, string typeName, params string[] descriptions)
        {
            var value = entry as ValueEntry;
            Assert.IsNotNull(value);

            Assert.AreEqual(value.Name, name);
            Assert.AreEqual(value.TypeName, typeName);

            var actualDescriptions = value.Descriptions.ToArray();
            Assert.AreEqual(descriptions.Length, actualDescriptions.Length);

            for (var i = 0; i < actualDescriptions.Length; i++)
            {
                Assert.AreEqual(descriptions[i], actualDescriptions[i]);
            }
        }

        private void AssertObject(TypeEntry entry, string name, string typeName)
        {
            var obj = entry as ObjectEntry;
            Assert.IsNotNull(obj);

            Assert.AreEqual(name, obj.Name);
            Assert.AreEqual(typeName,obj.TypeName);
        }

        private void AssertList(TypeEntry entry, string name, string typeName)
        {
            var list = entry as ListEntry;
            Assert.IsNotNull(list);

            Assert.AreEqual(list.Name, name);
            Assert.AreEqual(list.TypeName, typeName);
        }

    }
}
