using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DomainDrivenTest.Demo;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.ServiceModel;

namespace CodeArt.DomainDrivenTest.Demo
{
    [TestClass]
    public class OtherTest : DomainStage
    {
      
        [TestMethod]
        public void TypeDefine1()
        {
            //const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte'},first:'person',menu:{person:{id:'long',name:'string,5'},name:'string,10',parent:'menu',childs:['menu']},others:[{id:'int',name:'string',person:'menu.person'}],values:['string,10'],others2:'others'}";
            const string code = "{id:'int',name:'ascii,10'}";
            var define = new AggregateRootDefine("Ad", code);

            var ps = define.Properties.ToArray();

            AssertPropertyValue(ps[0], "Id", typeof(int));
            AssertPropertyValue(ps[1], "Name", typeof(string));
        }


        [TestMethod]
        public void TypeDefine2()
        {
            const string code = "{id:'int',name:'ascii,10',person:{name:'string,10',sex:'byte',hourse:{address:'string,10'},son:'person'},values:['string,10'],menu:{person:{id:'long',name:'string,5'},name:'string,10',parent:'menu',childs:['menu']}}";
            var define = new AggregateRootDefine("Admin", code);

            var ps = define.Properties.ToArray();

            AssertPropertyValue(ps[0], "Id", typeof(int));
            AssertPropertyValue(ps[1], "Name", typeof(string));

            AssertPerson(ps[2]);
            AssertPropertyList(ps[3], "values", typeof(List<string>));
            AssertMenu(ps[4]);
        }

        [TestMethod]
        public void TypeDefine3()
        {
            const string code = "people:{id:'int',name:'ascii,10',son:'people',wife:{sex:'byte',father:{name:'string'},son:'people'}}";
            var define = new AggregateRootDefine("People", code);

            //var ps = define.Properties.ToArray();

            //AssertPropertyValue(ps[0], "Id", typeof(int));
            //AssertPropertyValue(ps[1], "Name", typeof(string));


            //AssertPropertyObject(ps[2], "Son", "People");

            //AssertPropertyObject(ps[3], "Wife", "PeopleWife");

            //var wifeProperties = DomainProperty.GetProperties(ps[3].PropertyType).ToArray();
            //AssertPropertyObject(wifeProperties[1], "Father", "PeopleWifeFather");
            //AssertPropertyObject(wifeProperties[2], "Son", "People");
        }


        private void AssertPropertyList(DomainProperty property, string propertyName, Type propertyType)
        {
            Assert.IsTrue(property.PropertyType.IsList());
            Assert.AreEqual(property.Name, property.Name);
            Assert.AreEqual(propertyType, property.PropertyType);

        }



        private void AssertPropertyValue(DomainProperty property, string propertyName, Type propertyType)
        {
            Assert.AreEqual(propertyName, property.Name);
            Assert.AreEqual(propertyType, property.PropertyType);
        }

        private void AssertPropertyObject(DomainProperty property, string propertyName, string propertyTypeName)
        {
            Assert.AreEqual(propertyName,property.Name);
            Assert.AreEqual(propertyTypeName, property.PropertyType.Name);
        }

        private void AssertPerson(DomainProperty property)
        {
            AssertPropertyObject(property, "Person", "AdminPerson");
            var ps = DomainProperty.GetProperties(property.PropertyType).ToArray();

            AssertPropertyValue(ps[0], "Name", typeof(string));
            AssertPropertyValue(ps[1], "Sex", typeof(byte));

            AssertHouse(ps[2]);

            var son = ps[3];
            AssertPropertyObject(son, "Son", "AdminPerson");
            var sonPS = DomainProperty.GetProperties(son.PropertyType).ToArray();

            AssertPropertyValue(sonPS[0], "Name", typeof(string));
            AssertPropertyValue(sonPS[1], "Sex", typeof(byte));
            AssertHouse(sonPS[2]);
        }

        private void AssertHouse(DomainProperty property)
        {
            AssertPropertyObject(property, "Hourse", "AdminPersonHourse");
            var ps = DomainProperty.GetProperties(property.PropertyType).ToArray();

            AssertPropertyValue(ps[0], "Address", typeof(string));
        }


        private void AssertMenu(DomainProperty property)
        {
            //menu:{person:{id:'long',name:'string,5'},name:'string,10',parent:'menu',childs:['menu']}
            AssertPropertyObject(property, "Menu", "AdminMenu");
            var ps = DomainProperty.GetProperties(property.PropertyType).ToArray();


            AssertPropertyObject(ps[0], "Person", "AdminMenuPerson");
            var person = ps[0];
            var personProperties = DomainProperty.GetProperties(person.PropertyType).ToArray();
            AssertPropertyValue(personProperties[0], "Id", typeof(long));
            AssertPropertyValue(personProperties[1], "Name", typeof(string));


            AssertPropertyValue(ps[1], "Name", typeof(string));
            AssertPropertyObject(ps[2], "Parent", "AdminMenu");


            var childsType = TypeDefine.GetListType(property.PropertyType);
            AssertPropertyList(ps[3], "Childs", childsType);
        }
    }
}
