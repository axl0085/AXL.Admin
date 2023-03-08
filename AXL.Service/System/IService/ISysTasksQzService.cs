using AXL.Model.System;

namespace AXL.Service.System.IService {

    public interface ISysTasksQzService : IBaseService<SysTasks> {

        //SysTasksQz GetId(object id);
        int AddTasks(SysTasks parm);

        int UpdateTasks(SysTasks parm);
    }
}