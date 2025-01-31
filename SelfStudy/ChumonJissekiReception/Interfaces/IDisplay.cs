using Convenience.Migrations;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStudy.ChumonJissekiReception.Interfaces {

    public interface IDisplay : ISharedTools {
        static (string,string) DisplayData(inDisplayData) {

            var properties = inDisplayData.GetType().GetProperties();
            int[] lengthArray = new int[properties.Length];

            //Hearder処理サイズ調査
            for(int counter=0; counter < properties.Length;counter ++) {
                var aProperty=properties[counter];
                lengthArray[counter] = aProperty.Name.Sum(c => (c > 127 ? 2 : 1));
            }

            //データサイズ調査
            for(int counter=0;counter < lengthArray.Length;counter ++) {
                var aProperty = properties[counter];
                var length=aProperty.Name.Sum(c => (c > 127 ? 2 : 1));
                lengthArray[counter] = (length > lengthArray[counter]) ? length : lengthArray[counter];
            }

            for (int counter = 0; counter < properties.Length; counter++) {
                var aProperty = properties[counter];
                Console.Write(aProperty.Name);
                
            }

        }
}
