import { Module } from '@nestjs/common';
import { CacheModule } from '@nestjs/cache-manager';
import { EventEmitterModule } from '@nestjs/event-emitter';
import {
  AuthenticationModule,
  GroupModule,
  HealthModule,
  MetricSettingsModule,
  UploadModule,
  IndicatorModule,
  SectorModule,
  HierarchyModule,
  CollaboratorModule,
  ResultModule,
  PermissionModule,
  ProfileModule,
  MeModule,
  ProductModule,
  StockModule,
  CollaboratorVoucherModule,
  MonetizationModule,
  CustomScheduleModule,
  SupplierModule,
  CategoryModule,
  HolidaysModule,
} from './packages';
import { PrismaModule } from '~prisma/module';
import { LocalUploadProvider } from './infra/upload';
import { OrderModule } from './packages/marketplace/modules/order';
import { ScheduleModule } from '@nestjs/schedule';
import { ShoppingCartModule } from './packages/marketplace/modules/shopping-cart';
import { SizeModule } from './packages/marketplace/modules/size';
import { ProductDetailModule } from './packages/marketplace/modules/product-detail';
import { DetailsModule } from './packages/marketplace/modules/details';
import { SizesModule } from './packages/marketplace/modules/sizes';
import { TypesModule } from './packages/marketplace/modules/types';
import { ProductGroupModule } from './packages/marketplace/modules/product-group';
import { ProductColorModule } from './packages/marketplace/modules/product-color';

@Module({
  imports: [
    UploadModule.register(LocalUploadProvider),
    CacheModule.register({ isGlobal: true }),
    EventEmitterModule.forRoot(),
    ScheduleModule.forRoot(),
    HealthModule,
    PrismaModule,
    AuthenticationModule,
    GroupModule,
    SectorModule,
    MetricSettingsModule,
    IndicatorModule,
    HierarchyModule,
    CollaboratorModule,
    ResultModule,
    PermissionModule,
    ProfileModule,
    MeModule,
    ProductModule,
    StockModule,
    OrderModule,
    DetailsModule,
    SizesModule,
    TypesModule,
    ProductColorModule,
    SupplierModule,
    HolidaysModule,
    CategoryModule,
    CollaboratorVoucherModule,
    MonetizationModule,
    CustomScheduleModule.register(),
    ShoppingCartModule,
    SizeModule,
    ProductDetailModule,
    ProductGroupModule,
  ],
  controllers: [],
  providers: [],
  exports: [],
})
export class AppModule {}
