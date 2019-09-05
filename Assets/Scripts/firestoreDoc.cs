using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]



public class FirestoreDoc
{
    public _Ref _ref ;
    public _Fieldsproto _fieldsProto ;
    public _Serializer1 _serializer ;
    public _Readtime _readTime ;
    public _Createtime _createTime ;
    public _Updatetime _updateTime ;
}

public class _Ref
{
    public _Firestore _firestore ;
    public _Path _path ;
}

public class _Firestore
{
    public _Settings _settings ;
    public bool _settingsFrozen ;
    public _Serializer _serializer ;
    public string _projectId ;
    public long _lastSuccessfulRequest ;
    public bool _preferTransactions ;
    public _Clientpool _clientPool ;
}

public class _Settings
{
    public string projectId ;
    public string firebaseVersion ;
    public string libName ;
    public string libVersion ;
}

public class _Serializer
{
    public bool timestampsInSnapshots ;
}

public class _Clientpool
{
    public int concurrentOperationLimit ;
    public Activeclients activeClients ;
}

public class Activeclients
{
}

public class _Path
{
    public string[] segments ;
    public string projectId ;
    public string databaseId ;
}

public class _Fieldsproto
{
    public Id id ;
}

public class Id
{
    public string stringValue ;
    public string valueType ;
}

public class _Serializer1
{
    public bool timestampsInSnapshots ;
}

public class _Readtime
{
    public int _seconds ;
    public int _nanoseconds ;
}

public class _Createtime
{
    public int _seconds ;
    public int _nanoseconds ;
}

public class _Updatetime
{
    public int _seconds ;
    public int _nanoseconds ;
}
