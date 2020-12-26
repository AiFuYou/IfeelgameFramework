//
//  NativeAPI.m
//  Ifeelgame
//
//  Created by ifeel on 2020/8/27.
//  Copyright © 2020 ifeel. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeAPI.h"
#import "BCCKeychain/BCCKeychain.h"

@implementation NativeAPI

static NativeAPI *_instance = nil;

+(NativeAPI*) Instance{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        NSLog(@"Creating NativeAPI Instance");
        _instance = [[NativeAPI alloc] init];
    });
    return _instance;
}

-(id)init {
    self = [super init];
    return self;
}

-(NSString*) GetRegionName{
    NSLocale *currentLocale = [NSLocale currentLocale];
    NSString *countryCode = [currentLocale objectForKey:NSLocaleCountryCode];
    return [countryCode uppercaseString];
}

- (NSString*) GetUUID{
    NSString* userName = @"UUID";
    NSString* serviceName = [[NSBundle mainBundle]bundleIdentifier];
    
    NSString* uuid = [BCCKeychain getPasswordStringForUsername:userName andServiceName:serviceName error:nil];
    if (uuid == nil){
        uuid = [[NSUUID UUID] UUIDString];
        
        int retryCount = 0;
        int maxRetryCount = 3;
        while (![BCCKeychain storeUsername:userName andPasswordString:uuid forServiceName:serviceName updateExisting:false error:nil] && retryCount < maxRetryCount) {
            ++retryCount;
        }
    }
    
    return uuid;
}

@end
