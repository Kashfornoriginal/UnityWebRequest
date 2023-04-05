﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class WebRequestsService : IWebRequestsService
{
    public WebRequestsService(IUIFactory uiFactory, ILoggerService loggerService, ICoroutineRunner coroutineRunner)
    {
        _uiFactory = uiFactory;
        _loggerService = loggerService;
        _coroutineRunner = coroutineRunner;
    }

    private readonly IUIFactory _uiFactory;
    private readonly ILoggerService _loggerService;
    private readonly ICoroutineRunner _coroutineRunner;

    public void GetRequest(string url)
    { 
        _coroutineRunner.StartCoroutine(GetWebRequest(url, Success));
    }

    private void Success(List<Post> posts)
    {
        _uiFactory.ClearText();
        
        foreach (var post in posts)
        {
            _uiFactory.CreateText($"{post.id}: {post.title}");
        }
    }

    private IEnumerator GetWebRequest(string url, Action<List<Post>> success)
    {
        var webRequest = UnityWebRequest.Get(url);
        
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.InProgress:
                _loggerService.PrintInfo(nameof(WebRequestsService), "WebRequest is processing");
                break;
            case UnityWebRequest.Result.Success:
                _loggerService.PrintInfo(nameof(WebRequestsService), "WebRequest get success");
                
                var post = JsonConvert.DeserializeObject<List<Post>>(webRequest.downloadHandler.text);

                success?.Invoke(post);

                break;
            case UnityWebRequest.Result.ConnectionError:
                _loggerService.PrintWarning(nameof(WebRequestsService), "WebRequest connection error");
                break;
            case UnityWebRequest.Result.ProtocolError:
                _loggerService.PrintWarning(nameof(WebRequestsService), "WebRequest protocol error");
                break;
            case UnityWebRequest.Result.DataProcessingError:
                _loggerService.PrintWarning(nameof(WebRequestsService), "WebRequest data processing error");
                break;
            default:
                _loggerService.PrintWarning(nameof(WebRequestsService), "Web request argument out of range");
                break;
        }
    }
}