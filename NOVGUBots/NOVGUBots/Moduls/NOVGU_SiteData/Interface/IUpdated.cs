using BotsCore.Moduls.Translate;
using System.Collections.Generic;
using System.Linq;

namespace NOVGUBots.Moduls.NOVGU_SiteData.Interface
{
    public interface IUpdated : IGetId, ITranslatable
    {
        /// <summary>
        /// Обновить обьект в соответствии с входными данными
        /// </summary>
        bool Update(IEnumerable<object> newData, ref List<object> updatedInfo)
        {
            var infoUpdate = Update(newData, GetData(), ref updatedInfo);
            if (infoUpdate.stateUpdated)
            {
                SetData(infoUpdate.newData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Получить данные обьекта
        /// </summary>
        IEnumerable<object> GetData();
        void SetData(IEnumerable<object> newData);

        /// <summary>
        /// Метод обновления данных
        /// </summary>
        public static (bool stateUpdated, IEnumerable<object> newData) Update(IEnumerable<object> newData, IEnumerable<object> oldData, ref List<object> updatedElem)
        {
            bool resul = false;
            List<object> NewList = new();
            List<object> ThisList = oldData?.ToList();
            if (updatedElem == default)
                updatedElem = new List<object>();
            if (newData != null)
            {
                foreach (var elem in newData)
                {
                    object Identy = ThisList?.FirstOrDefault(x => ((IGetId)x).Similarity(elem));
                    if (Identy != null)
                    {
                        if (elem is IUpdated updated)
                            resul |= ((IUpdated)Identy).Update(updated.GetData(), ref updatedElem);
                        NewList.Add(Identy);
                        ThisList?.Remove(Identy);
                    }
                    else
                    {
                        NewList.Add(elem);
                        updatedElem.Add(elem);
                        resul = true;
                    }
                }
            }
            if (ThisList != null && ThisList.Count > 0)
                resul = true;
            if (resul)
                return (resul, NewList);
            return (resul, null);
        }
    }
}
