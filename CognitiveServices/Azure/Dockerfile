FROM mcr.microsoft.com/azureml/inference-base-1804:20221111.v1

RUN conda create -n minimal \
    python=3.7.10 pip=20.2.4

ENV AZUREML_CONDA_ENVIRONMENT_PATH /opt/miniconda/envs/minimal

# Prepend path to AzureML conda environment
ENV PATH $AZUREML_CONDA_ENVIRONMENT_PATH/bin:$PATH

RUN pip install 'azureml-inference-server-http~=0.7.0' \
                'azureml-core==1.45.0' \
                'turicreate'

ENV LD_LIBRARY_PATH $AZUREML_CONDA_ENVIRONMENT_PATH/lib:$LD_LIBRARY_PATH

CMD [ "runsvdir", "/var/runit" ]