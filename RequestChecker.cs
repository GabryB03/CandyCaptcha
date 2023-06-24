using System.Collections.Generic;
using System.Threading;

public class RequestChecker
{
    public List<string> requestUUIDs;
    public ResourceSemaphore semaphore;

    public RequestChecker()
    {
        this.requestUUIDs = new List<string>();
        this.semaphore = new ResourceSemaphore();
    }

    public void AddRequestUUID(string requestUUID)
    {
        while (semaphore.IsResourceNotAvailable())
        {
            Thread.Sleep(250);
        }

        if (semaphore.IsResourceAvailable())
        {
            semaphore.LockResource();
            requestUUIDs.Add(requestUUID);
            semaphore.UnlockResource();
        }
    }

    public void DeleteRequestUUID(string requestUUID)
    {
        while (semaphore.IsResourceNotAvailable())
        {
            Thread.Sleep(250);
        }

        if (semaphore.IsResourceAvailable())
        {
            semaphore.LockResource();
            requestUUIDs.Remove(requestUUID);
            semaphore.UnlockResource();
        }
    }

    public bool IsRequestUUIDAdded(string requestUUID)
    {
        bool exists = false;

        while (semaphore.IsResourceNotAvailable())
        {
            Thread.Sleep(250);
        }

        if (semaphore.IsResourceAvailable())
        {
            semaphore.LockResource();

            foreach (string uuid in requestUUIDs)
            {
                if (uuid.Equals(requestUUID))
                {
                    exists = true;
                    break;
                }
            }

            semaphore.UnlockResource();
        }

        return exists;
    }
}