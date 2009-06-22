namespace QI4N.Framework.Runtime
{
    using System;
    using System.Collections.Generic;

    public class ModuleInstance : Module
    {
        private readonly TransientBuilderFactory compositeBuilderFactory;

        private readonly IDictionary<Type, TransientFinder> compositeFinders;

        private readonly TransientsInstance transients;

        private readonly EntitiesInstance entities;

        private readonly IDictionary<Type, EntityFinder> entityFinders;


        private readonly ImportedServicesInstance importedServices;

        private readonly LayerInstance layerInstance;

        private readonly ModuleModel model;

        private readonly ObjectBuilderFactory objectBuilderFactory;

        private readonly IDictionary<Type, ObjectFinder> objectFinders;

        private readonly ObjectsInstance objects;

        private readonly ServiceFinderInstance serviceFinder;

        private readonly ServicesInstance services;

        private readonly UnitOfWorkFactoryInstance unitOfWorkFactory;

        private readonly ValueBuilderFactory valueBuilderFactory;

        private readonly IDictionary<Type, ValueFinder> valueFinders;

        private readonly ValuesInstance values;


        public ModuleInstance(ModuleModel moduleModel, LayerInstance layerInstance, TransientsModel transientsModel,
                              EntitiesModel entitiesModel, ObjectsModel objectsModel, ValuesModel valuesModel,
                              ServicesModel servicesModel, ImportedServicesModel importedServicesModel)
        {
            this.model = moduleModel;
            this.layerInstance = layerInstance;
            this.transients = new TransientsInstance(transientsModel, this);
            this.entities = new EntitiesInstance(entitiesModel, this);
            this.objects = new ObjectsInstance(objectsModel, this);
            this.values = new ValuesInstance(valuesModel, this);
            this.services = servicesModel.NewInstance(this);
            this.importedServices = importedServicesModel.NewInstance(this);

            this.compositeBuilderFactory = new TransientBuilderFactoryInstance(this);
            this.objectBuilderFactory = new ObjectBuilderFactoryInstance();
            this.valueBuilderFactory = new ValueBuilderFactoryInstance(this);
            this.unitOfWorkFactory = new UnitOfWorkFactoryInstance();
            this.serviceFinder = new ServiceFinderInstance();

            this.entityFinders = new Dictionary<Type, EntityFinder>();
            this.compositeFinders = new Dictionary<Type, TransientFinder>();
            this.objectFinders = new Dictionary<Type, ObjectFinder>();
            this.valueFinders = new Dictionary<Type, ValueFinder>();
        }

        public ModuleModel Model
        {
            get
            {
                return this.model;
            }
        }


        public TransientFinder FindCompositeModel(Type mixinType)
        {
            TransientFinder finder;
            if (!this.compositeFinders.TryGetValue(mixinType, out finder))
            {
                finder = new TransientFinder
                             {
                                     MixinType = mixinType
                             };
                this.VisitModules(finder);
                if (finder.Model != null)
                {
                    this.compositeFinders.Add(mixinType, finder);
                }
            }

            return finder;
        }

        public EntityFinder FindEntityModel(Type mixinType)
        {
            EntityFinder finder;
            if (!this.entityFinders.TryGetValue(mixinType, out finder))
            {
                finder = new EntityFinder
                             {
                                     MixinType = mixinType
                             };
                this.VisitModules(finder);
                if (finder.Model != null)
                {
                    this.entityFinders.Add(mixinType, finder);
                }
            }

            return finder;
        }

        public void VisitModules(ModuleVisitor visitor)
        {
            // Visit this module
            if (!visitor.VisitModule(this, this.Model, Visibility.Module))
            {
                return;
            }

            // Visit layer
            this.layerInstance.VisitModules(visitor, Visibility.Layer);
        }

        public ValueFinder FindValueModel(Type mixinType)
        {
            ValueFinder finder;
            if (!this.valueFinders.TryGetValue(mixinType, out finder))
            {
                finder = new ValueFinder
                {
                    MixinType = mixinType
                };
                this.VisitModules(finder);
                if (finder.Model != null)
                {
                    this.valueFinders.Add(mixinType, finder);
                }
            }

            return finder;
        }
    }

    public class ImportedServicesInstance
    {
    }

    public class ServicesInstance
    {
    }





    public class UnitOfWorkFactoryInstance
    {
    }

    public class ServiceFinderInstance
    {
    }

    public class ObjectFinder
    {
    }


    public interface Module
    {
    }
}